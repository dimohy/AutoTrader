using AutoTrader.Core.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTrader.Core.StockProvider.Stock
{
    public record 주식계좌정보
    {
        public string 계좌번호 { get; init; }

        public decimal 예수금 { get; init; }
        public decimal 유가잔고평가액 { get; init; }
        public decimal 예탁자산평가액 { get; init; }
        public decimal 총매입금액 { get; init; }
        public decimal 추정예탁자산 { get; init; }

        public IReadOnlyList<주식보유종목정보> 보유종목_목록 { get; init; }
    }

    public record 주식보유종목정보
    {
        public string 종목코드 { get; init; }
        public string 종목명 { get; init; }

        public int 보유수량 { get; init; }
        public float 평균단가 { get; init; }
        public float 현재가 { get; init; }
        public decimal 평가금액 => (decimal)현재가 * 보유수량;
        public decimal 손익금액 => 평가금액 - 매입금액;
        public float 손익율 => Calc.PerRate(현재가, 평균단가);
        public decimal 매입금액 { get; init; }

        public IReadOnlyList<주식주문정보> 체결상세_목록 { get; init; }
    }
}
