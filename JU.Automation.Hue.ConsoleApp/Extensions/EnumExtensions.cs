using System;
using System.Collections.Generic;
using System.Linq;

namespace JU.Automation.Hue.ConsoleApp.Extensions
{
    public static class EnumExtensions
    {
        public static IEnumerable<TEnum> GetUniqueFlags<TEnum>(this TEnum flags) where TEnum : Enum
        {
            ulong flag = 1;
            foreach (var value in Enum.GetValues(flags.GetType()).Cast<TEnum>())
            {
                ulong bits = Convert.ToUInt64(value);
                while (flag < bits)
                {
                    flag <<= 1;
                }

                if (flag == bits && flags.HasFlag(value))
                {
                    yield return value;
                }
            }
        }
    }
}
