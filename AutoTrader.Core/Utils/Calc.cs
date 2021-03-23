using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTrader.Core.Utils
{
    public static class Calc
    {
        public static decimal PerRate(decimal a, decimal b) => (a - b) / b * 100m;
        public static float PerRate(float a, float b) => (a - b) / b * 100f;
    }
}
