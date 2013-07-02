using System;
using System.ComponentModel;
using System.Reflection;

namespace SomethingBlue.Helpers
{
    public class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        private PropertyInfo _nameProperty;
        private Type _resourceType;

        public LocalizedDescriptionAttribute(string displayNameKey)
            : base(displayNameKey)
        {
        }

        public Type ResourceType
        {
            get { return _resourceType; }
            set
            {
                _resourceType = value;
                _nameProperty = _resourceType.GetProperty(this.Description, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            }
        }

        public override string Description
        {
            get
            {
                // Check if nameProperty is null and return original display name value.
                if (_nameProperty == null)
                    return base.Description;

                return (string)_nameProperty.GetValue(_nameProperty.DeclaringType, null);
            }
        }
    }

}
