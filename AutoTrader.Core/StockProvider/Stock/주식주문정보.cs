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
        public string 계좌번호 { get; init; }
        public string 종목코드 { get; init; }
        public 주문유형 주문유형 { get; init; }
        public 거래구분 거래구분 { get; init; }
        public float 주문수량 { get; init; }
        public float 주문가격 { get; init; }
        public string 원주문번호 { get; init; }
        public 주식주문정보 원주문 { get; init; }
        public DateTime 주문일시 { get; init; }

        public string 주문메시지 { get; init; }

        public bool 정정주문유무 => string.IsNullOrWhiteSpace(원주문번호) == false;
        public bool 주문성공유무 => string.IsNullOrWhiteSpace(주문번호) == false;
        //public bool 주문성공유무 => 주문번호 != 0;
    }
}
