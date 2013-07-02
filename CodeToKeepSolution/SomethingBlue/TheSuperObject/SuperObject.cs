using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SomethingBlue.TheSuperObject
{
    [Serializable]
    public class SuperObject : DynamicObject, IDataErrorInfo
    {
        private List<PropertyFilterManager> PropertyFilters { get; set; }
        private List<MultiSelectStrategy> MultiSelectStrategies { get; set; }
        private List<object> Objects { get; set; }
        private List<PropertyInfo> CommonProperties { get; set; }
        private Dictionary<string, string> Errors { get; set; }

        public SuperObject(IEnumerable<Object> objects) : this(objects, null, null)
        {
        }

        public SuperObject(IEnumerable<Object> objects, List<PropertyFilterManager> propertyFilters, List<MultiSelectStrategy> multiSelectStrategies)
        {
            PropertyFilters = propertyFilters ?? new List<PropertyFilterManager>();
            MultiSelectStrategies = multiSelectStrategies ?? new List<MultiSelectStrategy>();
            Errors = new Dictionary<string, string>();
            Objects = new List<object>();
            Objects.AddRange(objects);
            SetupObjects();
        }

        private void SetupObjects()
        {
            DynamicMembers = new Dictionary<string, object>();

            if (Objects.Count == 0) return;

            CommonProperties = ReflectionHelpers.GetCommonProperties(Objects);
            if (PropertyFilters.Count > 0)
                CommonProperties = ReflectionHelpers.FilterCommonProperties(CommonProperties, PropertyFilters);

            bool useMultiSelectStrategy = MultiSelectStrategies.Count > 0 && Objects.Count() > 1;
            MultiSelectStrategy multiSelectStrategy = null;

            foreach (var info in CommonProperties.Where(info => info.CanRead))
            {
                object valueToSet = null;

                if (useMultiSelectStrategy)
                    multiSelectStrategy = MultiSelectStrategies.FirstOrDefault(x => x.PropertyName == info.Name);

                if (multiSelectStrategy != null)
                {
                    var strategy = multiSelectStrategy.Strategy;

                    if (strategy == StrategyType.Hide)
                        continue;

                    if (strategy == StrategyType.Show || strategy == StrategyType.ShowAndValidate)
                        valueToSet = multiSelectStrategy.DefaultValue;
                }
                valueToSet = ReflectionHelpers.GetDefaultValue(info, Objects, valueToSet);
                DynamicMembers[info.Name] = valueToSet;
                UpdateErrorInfoFromOrginalObject(info);
            }
        }


        private void UpdateErrorInfoFromOrginalObject(PropertyInfo prop)
        {
            string error = null;
            foreach (var o in Objects)
            {
                var info = o as IDataErrorInfo;
                if (info != null)
                {
                    error = info[prop.Name];
                    if (string.IsNullOrEmpty(error) == false)
                    {
                        // add error to local error collection
                        Errors[prop.Name] = error;
                    }
                }
            }
            // remove previous error from error collection
            if (string.IsNullOrEmpty(error))
                Errors.Remove(prop.Name);
        }

        private void ValidateRuleForProperty(PropertyInfo prop, object value)
        {
            string error = null;
            foreach (var o in Objects)
            {

                var orgValue = prop.GetValue(o, null);
                // check if input value is of the right type

                prop.SetValue(o, value, null);
                // now check for errors
                var info = o as IDataErrorInfo;
                if (info != null)
                {
                    error = info[prop.Name];
                    if (string.IsNullOrEmpty(error) == false)
                    {
                        // add error to local error collection
                        Errors[prop.Name] = error;
                        // reset value on object
                        //prop.SetValue(o, orgValue, null);
                        //var field = o as PropertyDefinition;
                        //if (field != null)
                        //    field.IsChanged = false;
                        break;
                    }
                }
            }
            // remove previous error from error collection
            if (string.IsNullOrEmpty(error))
                Errors.Remove(prop.Name);
        }

        #region DynamicObject overrides

        [Browsable(false)]
        public IDictionary<string, object> DynamicMembers { get; private set; }

        [Browsable(false)]
        public int Count
        {
            get { return DynamicMembers.Keys.Count; }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (DynamicMembers.ContainsKey(binder.Name))
            {
                result = DynamicMembers[binder.Name];
                return true;
            }
            return base.TryGetMember(binder, out result); //means result = null and return = false
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (DynamicMembers.ContainsKey(binder.Name))
            {
                var prop = CommonProperties.First(x => x.Name == binder.Name);
                try
                {
                    //Convert.ChangeType does not handle conversion to nullable types
                    //if the property type is nullable, we need to get the underlying type of the property
                    if (value != null)
                    {
                        Type targetType = IsNullableType(prop.PropertyType)
                            ? Nullable.GetUnderlyingType(prop.PropertyType)
                            : prop.PropertyType;
                        value = Convert.ChangeType(value, targetType);
                    }
                }
                catch (Exception)
                {
                    Debug.WriteLine("Falied to set value: " + value);
                    return false;
                }
                DynamicMembers[binder.Name] = value;
                ValidateRuleForProperty(prop, value);
            }
            else
                DynamicMembers.Add(binder.Name, value);

            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if (DynamicMembers.ContainsKey(binder.Name) && DynamicMembers[binder.Name] is Delegate)
            {
                result = (DynamicMembers[binder.Name] as Delegate).DynamicInvoke(args);
                return true;
            }
            return base.TryInvokeMember(binder, args, out result);
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return DynamicMembers.Keys;
        }

        #endregion DynamicObject overrides

        #region IDataErrorInfo

        [Browsable(false)]
        public string this[string columnName]
        {
            get { return Errors.ContainsKey(columnName) ? Errors[columnName] : null; }
        }

        [Browsable(false)]
        public string Error
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var error in Errors.Values)
                    sb.AppendLine(error);

                return sb.ToString();
            }
        }

        #endregion IDataErrorInfo

        #region helpers

        
        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        } 

        #endregion helpers
    }
}