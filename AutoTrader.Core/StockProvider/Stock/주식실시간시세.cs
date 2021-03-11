using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTrader.Core.StockProvider.Stock
{
    public record 주식실시간시세
    {
        public string 종목코드 { get; init; }

        public float 매수1호가 { get; init; }
        public int 매수1호가잔량 { get; init; }
        public float 매수2호가 { get; init; }
        public int 매수2호가잔량 { get; init; }
        public float 매수3호가 { get; init; }
        public int 매수3호가잔량 { get; init; }
        public float 매수4호가 { get; init; }
        public int 매수4호가잔량 { get; init; }
        public float 매수5호가 { get; init; }
        public int 매수5호가잔량 { get; init; }
        public float 매수6호가 { get; init; }
        public int 매수6호가잔량 { get; init; }
        public float 매수7호가 { get; init; }
        public int 매수7호가잔량 { get; init; }
        public float 매도1호가 { get; init; }
        public int 매도1호가잔량 { get; init; }
        public float 매도2호가 { get; init; }
        public int 매도2호가잔량 { get; init; }
        public float 매도3호가 { get; init; }
        public int 매도3호가잔량 { get; init; }
        public float 매도4호가 { get; init; }
        public int 매도4호가잔량 { get; init; }
        public float 매도5호가 { get; init; }
        public int 매도5호가잔량 { get; init; }
        public float 매도6호가 { get; init; }
        public int 매도6호가잔량 { get; init; }
        public float 매도7호가 { get; init; }
        public int 매도7호가잔량 { get; init; }

        public bool 체결유무 { get; init; }
        public 주식체결방향 체결방향 { get; init; }
        public float 체결가 { get; init; }
        public float 현재가 => 체결가;
        public int 거래량 { get; init; }
        public int 누적거래량 { get; init; }
        public float 시가 { get; init; }
        public float 고가 { get; init; }
        public float 저가 { get; init; }

        public DateTime 시각 { get; init; }
    }

    public enum 주식체결방향
    {
        상승,
        하락
    }
}
