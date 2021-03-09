using AutoTrader.Core.StockProvider.Common;
using AutoTrader.Core.StockProvider.Stock;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTrader.Core.StockProvider
{
    public interface IStockProvider
    {
        Task<사용자정보> 로그인(string id, string passwords, ExtraLoginParams loginParams);
        //Task LogoutAsync();
        bool 연결_유무();

        ///////////////
        // 주식 관련 //
        ///////////////

        // ### 조회 기능
        Task<주식기본정보> 주식_기본정보_조회(string 종목코드);
        Task<IReadOnlyList<주식일봉정보>> 주식_일봉정보_조회(string 종목코드, DateTime 기준일자);

        Task<IReadOnlyList<주식분봉정보>> 주식_분봉_조회(string 종목코드, 분봉구분 분봉구분);

        // ### 실시간 기능

        // ### 매매 기능
        Task<주식주문정보> 주식_주문(string 계좌번호, string 종목코드, 주문유형 주문유형, float 가격, float 수량, 거래구분 거래구분);
        Task<주식주문정보> 주식_주문정정(주식주문정보 원주문, float 가격, float 수량);
        Task<bool> 주식_주문취소(주식주문정보 주문);


        ////////////////////
        // 선물옵션 관련 //
        ////////////////////

        // ### 조회 기능

        // ### 실시간 기능

        // ### 매매 기능


        ///////////////////////
        // 해외선물옵션 관련 //
        ///////////////////////

        // ### 조회 기능

        // ### 실시간 기능

        // ### 매매 기능


        ///////////////////
        // 가상화폐 관련 //
        ///////////////////

        // ### 조회 기능

        // ### 실시간 기능

        // ### 매매 기능
    }

    public record ExtraLoginParams();

    public enum 분봉구분
    {
        분봉_1분봉,
        분봉_5분봉,
        분봉_10분봉,
        분봉_15분봉,
        분봉_30분봉,
        분봉_45분봉,
        분봉_60분봉
    }

    public enum 주문유형
    {
        매수,
        매도
    }

    public enum 거래구분
    {
        지정가,
        시장가
    }
}

