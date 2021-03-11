using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTrader.Core.Extensions
{
    public static class StringExtension
    {
        public static TResult CastTo<TResult>(this string @this)
        {
            if (@this == null || @this.Length == 0)
                return default;

            if (typeof(TResult) == typeof(string))
                return (TResult)(object)@this.Trim();

            @this = @this.Trim();
            var type = typeof(TResult);

            if (type == typeof(DateTime))
            {
                var now = DateTime.Now;

                // 6자리 시분초
                if (@this.Length == 6)
                    return (TResult)(object)(new DateTime(now.Year, now.Month, now.Day, int.Parse(@this[..2]), int.Parse(@this[2..4]), int.Parse(@this[4..])));
                // 8자리 연월일
                else if (@this.Length == 8)
                    return (TResult)(object)(new DateTime(int.Parse(@this[..4]), int.Parse(@this[4..6]), int.Parse(@this[6..8])));
                // 14자리 연월일시분초
                else if (@this.Length == 14)
                    return (TResult)(object)(new DateTime(int.Parse(@this[..4]), int.Parse(@this[4..6]), int.Parse(@this[6..8]), int.Parse(@this[8..10]), int.Parse(@this[10..12]), int.Parse(@this[12..])));
            }

            return (TResult)Convert.ChangeType(@this, type);
        }
    }
}
