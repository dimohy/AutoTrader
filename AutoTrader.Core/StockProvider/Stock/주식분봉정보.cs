using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTrader.Core.StockProvider.Stock
{
    public record 주식분봉정보
    {
        public string 종목코드 { get; init; }
        public float 현재가 { get; init; }
        public int 거래량 { get; init; }
        public DateTime 체결시간 { get; init; }
        public float 시가 { get; init; }
        public float 고가 { get; init; }
        public float 저가 { get; init; }
    }
}
