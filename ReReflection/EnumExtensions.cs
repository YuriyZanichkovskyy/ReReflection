using System;
using System.Collections.Generic;
using JetBrains.Extension;

namespace ReSharper.Reflection
{
    public static class EnumExtensions
    {
        public static string GetFullString(this Enum enumeration, bool includeZeroValueForFlags = false)
        {
            var enumType = enumeration.GetType();
            if (enumType.GetCustomAttribute<FlagsAttribute>(true) != null)
            {
                IList<string> setFlags = new List<string>();

                var enumValues = Enum.GetValues(enumType);

                for (int i = 0; i < enumValues.Length; i++)
                {
                    if((int)enumValues.GetValue(i) == 0)
                        continue;

                    if (enumeration.HasFlag((Enum)enumValues.GetValue(i)))
                    {
                        setFlags.Add(string.Format("{0}.{1}", enumType.Name, enumValues.GetValue(i)));
                    }
                }

                return string.Join(" | ", setFlags);
            }

            return string.Format("{0}.{1}", enumType.Name, enumeration);
        }
    }
}
