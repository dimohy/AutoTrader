using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTrader.Core.Extensions
{
    public static class NumberExtension
    {
        public static float Abs(this float @this) => Math.Abs(@this);
        public static int Abs(this int @this) => Math.Abs(@this);
    }
}
