using AutoTrader.Core.StockProvider;
using AutoTrader.Core.StockProvider.Common;
using AutoTrader.Core.StockProvider.Stock;

using AxKHOpenAPILib;

using KHOpenAPILib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoTrader.StockProvider.Kiwoom
{
    public sealed class SessionManager : IStockProvider
    {
        private readonly Thread messageThread;
        private IStockProvider openApi;

        public Action<주식실시간시세> 주식_실시간시세_호출 { get => openApi.주식_실시간시세_호출; set => openApi.주식_실시간시세_호출 = value; }

        public SessionManager()
        {
            var initResetEvent = new ManualResetEvent(false);

            messageThread = new Thread(() =>
            {
                var openApiForm = new OpenApiForm
                {
                    InitCompleteEvent = initResetEvent
                };
                openApi = openApiForm;

                Application.Run(openApiForm);
            })
            {
                IsBackground = true
            };
            messageThread.SetApartmentState(ApartmentState.STA);
            messageThread.Start();

            initResetEvent.WaitOne();
        }

        public bool 연결_유무()
        {
            return openApi.연결_유무();
        }

        public Task<사용자정보> 로그인(string id = default, string passwords = default, ExtraLoginParams loginParams = default)
        {
            return openApi.로그인(id, passwords, loginParams);
        }

        public Task<주식기본정보> 주식_기본정보_조회(string 종목코드)
        {
            return openApi.주식_기본정보_조회(종목코드);
        }

        public Task<IReadOnlyList<주식일봉정보>> 주식_일봉정보_조회(string 종목코드, DateTime 기준일자)
        {
            return openApi.주식_일봉정보_조회(종목코드, 기준일자);
        }

        public Task<IReadOnlyList<주식분봉정보>> 주식_분봉_조회(string 종목코드, 분봉구분 분봉구분)
        {
            return openApi.주식_분봉_조회(종목코드, 분봉구분);
        }

        public Task<주식주문정보> 주식_주문(string 계좌번호, string 종목코드, 주문유형 주문유형, float 가격, int 수량, 거래구분 거래구분)
        {
            return openApi.주식_주문(계좌번호, 종목코드, 주문유형, 가격, 수량, 거래구분);
        }

        public Task<주식주문정보> 주식_주문정정(주식주문정보 원주문, float 가격, int 수량)
        {
            return openApi.주식_주문정정(원주문, 가격, 수량);
        }

        public Task<주식주문정보> 주식_주문취소(주식주문정보 주문)
        {
            return openApi.주식_주문취소(주문);
        }

        public Task 주식_실시간시세_등록(params string[] 종목코드_목록)
        {
            return openApi.주식_실시간시세_등록(종목코드_목록);
        }

        public Task 주식_실시간시세_해제(params string[] 종목코드_목록)
        {
            return openApi.주식_실시간시세_해제(종목코드_목록);
        }

        public Task 주식_실시간시세_전체해제()
        {
            return openApi.주식_실시간시세_전체해제();
        }

        public Task<주식계좌정보> 주식_계좌정보_조회(string 계좌번호)
        {
            return openApi.주식_계좌정보_조회(계좌번호);
        }
    }
}
