using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTrader.Core.Extensions
{
    public static class DateTimeExtension
    {
        public static string ToCombineDate(this DateTime @this)
        {
            return @this.ToString("yyyyMMdd");
        }
    }
}
