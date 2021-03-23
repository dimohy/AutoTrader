using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTrader.Core.StockProvider.Stock
{
    public record 주식주문정보
    {
        public string 주문번호 { get; init; }
        public 주문상태 주문상태 { get; init; }
        public string 계좌번호 { get; init; }
        public string 종목코드 { get; init; }
        public string 종목명 { get; init; }
        public 주문유형 주문유형 { get; init; }
        public 거래구분 거래구분 { get; init; }
        public int 주문수량 { get; init; }
        public float 주문가격 { get; init; }
        public string 원주문번호 { get; init; }
        public 주식주문정보 원주문 { get; init; }
        public DateTime 주문일시 { get; init; }

        public float 체결가 { get; init; }
        public int 체결수량 { get; init; }
        public int 미체결수량 { get; init; }
        public bool 체결완료유무 => 미체결수량 == 0;
        public decimal 매매수수료 { get; init; }
        public decimal 매매세금 { get; init; }

        public string 메시지 { get; init; }

        public bool 취소유무 { get; init; }
        public bool 정정주문유무 => string.IsNullOrWhiteSpace(원주문번호) == false;
        public bool 처리유무 => string.IsNullOrWhiteSpace(주문번호) == false;
    }
}
