using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RxFair.Utility.Helpers
{
    public static class EnumHelpers
    {
        public class NameValue
        {
            public string Name { get; set; }
            public object Value { get; set; }
        }

        public static List<NameValue> EnumToList<T>()
        {
            var valueList = (T[])(Enum.GetValues(typeof(T)).Cast<T>());
            var nameList = Enum.GetNames(typeof(T)).ToArray();
            List<NameValue> lst = null;
            for (var i = 0; i < valueList.Length; i++)
            {
                if (lst == null)
                    lst = new List<NameValue>();
                var name = nameList[i];
                var value = (i + 1);
                lst.Add(new NameValue { Name = name, Value = value });
            }
            return lst;
        }

        public static string GetEnumDescription<TEnum>(TEnum value)
        {
            var fi = value.GetType().GetField(value.ToString());

            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if ((attributes != null) && (attributes.Length > 0))
                return attributes[0].Description;
            return value.ToString();
        }

        public static string GetDescription(this Enum genericEnum)
        {
            var genericEnumType = genericEnum.GetType();
            var memberInfo = genericEnumType.GetMember(genericEnum.ToString());
            if ((memberInfo == null || memberInfo.Length <= 0)) return genericEnum.ToString();
            var attribs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            if ((attribs != null && attribs.Any()))
            {
                return ((DescriptionAttribute)attribs.ElementAt(0)).Description;
            }
            return genericEnum.ToString();
        }

        public static TEnum GetValueFromDescription<TEnum>(string description)
        {
            var type = typeof(TEnum);
            if (!type.IsEnum) throw new InvalidOperationException();
            foreach (var field in type.GetFields())
            {
                if (Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                        return (TEnum)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (TEnum)field.GetValue(null);
                }
            }
            throw new ArgumentException("Not found.", nameof(description));
        }

    }
}
