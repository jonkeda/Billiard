using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Billiard.Extensions
{
    public static class EnumExtension
    {
        public static string GetDisplayName(this Enum enumVal)
        {
            var display = enumVal.GetAttributeOfType<DisplayAttribute>();
            if (display != null
                && display.Name != null)
            {
                return display.Name;
            }
            return enumVal.ToString();
        }

        /// <summary>
        /// Gets an attribute on an enum field value
        /// </summary>
        /// <typeparam name="T">The type of the attribute you want to retrieve</typeparam>
        /// <param name="enumVal">The enum value</param>
        /// <returns>The attribute of type T that exists on the enum value</returns>
        /// <example><![CDATA[string desc = myEnumVariable.GetAttributeOfType<DescriptionAttribute>().Description;]]></example>
        public static T? GetAttributeOfType<T>(this Enum enumVal) where T : System.Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            if (memInfo.Length == 0)
            {
                return default;
            }
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);

            return (attributes.Length > 0) ? (T)attributes[0] : null;
        }

        public static List<T> Split<T>(string text)
            where T : struct 
        {
            List<T> list = new List<T>();
            if (string.IsNullOrEmpty(text))
            {
                return list;
            }
            string[] words = text.Split(',');
            foreach (string word in words)
            {
                if (Enum.TryParse(word, out T e))
                {
                    list.Add(e);
                }
            }

            return list;
        }
    }
}