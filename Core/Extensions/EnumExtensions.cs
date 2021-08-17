using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using bEmu.Core.Attributes;
using bEmu.Core.Util;

namespace bEmu.Core.Extensions
{
    public static class EnumExtensions
    {
        public static string GetEnumDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
                return attributes.First().Description;

            return value.ToString();
        }

        public static bool IsIgnore(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            IgnoreAttribute[] attributes = fi.GetCustomAttributes(typeof(IgnoreAttribute), false) as IgnoreAttribute[];

            return attributes != null && attributes.Any();
        }
    }
}