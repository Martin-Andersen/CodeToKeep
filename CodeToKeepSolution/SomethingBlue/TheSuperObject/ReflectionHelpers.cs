using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace SomethingBlue.TheSuperObject
{
    /// <summary>
    /// Thank's to 
    /// http://stackoverflow.com/questions/353430/easiest-way-to-get-a-common-base-class-from-a-collection-of-types/353672#353672 
    /// http://www.west-wind.com/weblog/posts/2012/Feb/08/Creating-a-dynamic-extensible-C-Expando-Object
    /// http://www.codeproject.com/Articles/100710/Using-DynamicObject-to-Implement-General-Proxy-Cla
    /// </summary>
    public static class ReflectionHelpers
    {
        public static List<PropertyInfo> FilterCommonProperties(List<PropertyInfo> infos, List<PropertyFilterManager> filters)
        {
            var localInfos = infos.ToList();

            foreach (var info in infos)
                foreach (var filterManager in filters)
                {
                    if (filterManager.IsPositivList && filterManager.PropertyNames.Contains(info.Name) == false)
                        localInfos.Remove(info);
                    else if (filterManager.IsPositivList == false && filterManager.PropertyNames.Contains(info.Name))
                        localInfos.Remove(info);

                }
            return localInfos;
        }

        public static List<PropertyInfo> GetCommonProperties(IEnumerable<object> items)
        {
            var types = (from x in items select x.GetType()).ToArray();
            return GetCommonProperties(types);
        }

        public static List<PropertyInfo> GetCommonProperties(Type[] types)
        {
            return (from t in types
                    from p in t.GetProperties()
                    group p by p.Name into pg
                    where pg.Count() == types.Length
                    select pg.ElementAt(0)).ToList();
        }


        public static Type GetCommonBaseClass(IEnumerable<object> items)
        {
            var types = (from x in items select x.GetType()).ToArray();
            return GetCommonBaseClass(types);
        }

        public static Type GetCommonBaseClass(Type[] types)
        {
            if (types.Length == 0)
                return typeof(object);

            Type ret = types[0];

            for (int i = 1; i < types.Length; ++i)
            {
                if (types[i].IsAssignableFrom(ret))
                    ret = types[i];
                else
                {
                    // This will always terminate when ret == typeof(object)
                    while (ret != null && !ret.IsAssignableFrom(types[i]))
                        ret = ret.BaseType;
                }
            }
            return ret;
        }

        public static object GetDefaultValue(PropertyInfo prop)
        {
            var attributes = prop.GetCustomAttributes(typeof(DefaultValueAttribute), true);
            if (attributes.Length > 0)
            {
                var defaultAttr = (DefaultValueAttribute)attributes[0];
                return defaultAttr.Value;
            }

            // Attribute not found, fall back to default value for the type
            if (prop.PropertyType.Name == "String")
                return string.Empty;

            return prop.PropertyType.IsValueType ? Activator.CreateInstance(prop.PropertyType) : null;
        }

        public static object GetDefaultValue(PropertyInfo prop, List<object> objects, object defaultValue = null)
        {
            if (defaultValue != null && defaultValue.GetType() == prop.DeclaringType)
                return defaultValue;

            var distinct = objects.Select(o => prop.GetValue(o, null)).ToList().Distinct().ToArray();
            return distinct.Count() == 1 ? distinct[0] : GetDefaultValue(prop);
        }
        
    }
}