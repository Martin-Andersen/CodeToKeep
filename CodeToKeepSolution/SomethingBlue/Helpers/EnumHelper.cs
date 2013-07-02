using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace SomethingBlue.Helpers
{
    /// <summary>
    /// Helper class for <see cref="Enum"/>s.
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// Parses the specified value.
        /// </summary>
        /// <typeparam name="T">Base Type Enum</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>The enum value matching the value provided.</returns>
        public static T Parse<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }

        /// <summary>
        /// Parses the specified description value.
        /// </summary>
        /// <typeparam name="T">Base Type Enum</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>The enum value matching the decription provided; otherwise the first item in the enum if not found.</returns>
        public static T ParseByDescription<T>(string value)
        {
            foreach (var name in Enum.GetNames(typeof(T)))
            {
                var enumValue = Parse<T>(name) as Enum;
                if (enumValue.GetDescription().Equals(value))
                    return Parse<T>(name);
            }

            return default(T);
        }

        /// <summary>
        /// Returns the text value of the DescriptionAttribute for the given enum.
        /// If no description attribute is found, the enum's .ToString() value is returned.
        /// </summary>
        /// <param name="enumeration">The enumeration.</param>
        /// <returns>The value from the <see cref="DescriptionAttribute"/>. If not available then performs a ToString().</returns>
        public static string GetDescription(this Enum enumeration)
        {
            var value = enumeration.ToString();
            var type = enumeration.GetType();
            var descAttribute = (DescriptionAttribute[]) type.GetField(value).GetCustomAttributes(typeof (DescriptionAttribute), false);
            
            return descAttribute.Length > 0 ? descAttribute[0].Description : value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enumeration"></param>
        /// <returns></returns>
        public static string GetLocalizedDescription(this Enum enumeration)
        {
            if (enumeration == null)
                return null;

            string description = enumeration.ToString();

            FieldInfo fieldInfo = enumeration.GetType().GetField(description);
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Any())
                return attributes[0].Description;

            return description;
        }
    }
}
