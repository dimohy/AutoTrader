using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTrader.Core.StockProvider.Stock
{
    public record 주식기본정보
    {
        public string 종목코드 { get; init; }
        public string 종목명 { get; init; }
        public int 결산월 { get; init; }
        public float 액면가 { get; init; }
        public decimal 자본금 { get; init; }
        public int 상장주식 { get; init; }
        public float 신용비율 { get; init; }
        public float 연중최고 { get; init; }
        public float 연중최저 { get; init; }
        public decimal 시가총액 { get; init; }
        public float 외인소진률 { get; init; }
        public decimal 대용가 { get; init; }
        public float PER { get; init; }
        public decimal EPS { get; init; }
        public float ROE { get; init; }
        public float PBR { get; init; }
        public float EV { get; init; }
        public decimal BPS { get; init; }
        public decimal 매출액 { get; init; }
        public decimal 영업이익 { get; init; }
        public decimal 당기순이익 { get; init; }
        public float N250최고 { get; init; }
        public float N250최저 { get; init; }
        public float 시가 { get; init; }
        public float 고가 { get; init; }
        public float 저가 { get; init; }
        public float 상한가 { get; init; }
        public float 하한가 { get; init; }
        public float 기준가 { get; init; }
        public float 예상체결가 { get; init; }
        public int 예상체결수량 { get; init; }
        public DateTime N250최고가일 { get; init; }
        public float N250최고가대비율 { get; init; }
        public DateTime N250최저가일 { get; init; }
        public float N250최저가대비율 { get; init; }
        public float 현재가 { get; init; }
        public int 대비기호 { get; init; }
        public float 전일대비 { get; init; }
        public float 등락율 { get; init; }
        public int 거래량 { get; init; }
        public float 거래대비 { get; init; }
        public string 액면가단위 { get; init; }
        public int 유통주식 { get; init; }
        public float 유통비율 { get; init; }
    };
}
