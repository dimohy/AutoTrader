using AutoTrader.Core.Extensions;
using AutoTrader.Core.StockProvider;
using AutoTrader.Core.StockProvider.Common;
using AutoTrader.Core.StockProvider.Stock;

using AxKHOpenAPILib;

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoTrader.StockProvider.Kiwoom
{
    public class OpenApiForm : Form, IStockProvider
    {
        private static readonly TimeSpan RequestTimeInterval = TimeSpan.FromSeconds(0.2d);

        private AxKHOpenAPI api;
        private readonly ManualResetEventSlim waitConnectResetEvent = new(false);
        private Exception lastException;
        private 사용자정보 사용자정보;
        private ushort reqNum;
        private readonly ConcurrentDictionary<string, ReqResult> reqNameToResultMap = new();
        private Stopwatch stopwatch;
        private TimeSpan lastRequestTimeSpan;
        private TimeSpan lastOrderTimeSpan;

        public ManualResetEvent InitCompleteEvent { private get; set; }

        public async Task<사용자정보> 로그인(string id = default, string passwords = default, ExtraLoginParams loginParams = default)
        {
            await Task.Run(() =>
            {
                waitConnectResetEvent.Reset();
                var result = 0;
                api.Invoke(new Action(() => result = api.CommConnect()));
                if (result < 0)
                    throw new StockProviderLoginException(StockProviderExceptionKind.로그인오류_통신장애);
                // 30초 동안 입력되지 않으면 로그인 실패
                var bResult = waitConnectResetEvent.Wait(30 * 1000);
                if (bResult == false)
                    throw new StockProviderLoginException(StockProviderExceptionKind.로그인오류_타임아웃);

                // 연결이 되지 않았으면 마지막 예외를 발생한다.
                if (연결_유무() == false)
                    throw lastException;

                stopwatch = Stopwatch.StartNew();
            });

            return 사용자정보;
        }

        //public async Task LogoutAsync()
        //{
        //    await Task.Run(() =>
        //    {
        //        api.Invoke(new Action(() => api.CommTerminate()));
        //    });
        //}

        protected override void OnLoad(EventArgs e)
        {
            Opacity = 0;
            ShowInTaskbar = false;

            api = new AxKHOpenAPI();
            // 연결시 호출
            api.OnEventConnect += Api_OnEventConnect;
            // 메시지 수신시 호출
            api.OnReceiveMsg += Api_OnReceiveMsg;
            //api.OnReceiveRealCondition += Api_OnReceiveRealCondition;
            //api.OnReceiveInvestRealData += Api_OnReceiveInvestRealData;
            // 체결/잔고변경 수신시 호출
            api.OnReceiveChejanData += Api_OnReceiveChejanData;
            // 실시간 수신시 호출
            api.OnReceiveRealData += Api_OnReceiveRealData;
            // TR 요청 수신시 호출
            api.OnReceiveTrData += Api_OnReceiveTrData;
            //api.OnReceiveTrCondition += Api_OnReceiveTrCondition;
            //api.OnReceiveConditionVer += Api_OnReceiveConditionVer;

            Controls.Add(api);

            base.OnLoad(e);

            InitCompleteEvent.Set();
        }

        //private void Api_OnReceiveConditionVer(object sender, _DKHOpenAPIEvents_OnReceiveConditionVerEvent e)
        //{
        //    Console.WriteLine($"Api_OnReceiveConditionVer: ");
        //}

        //private void Api_OnReceiveTrCondition(object sender, _DKHOpenAPIEvents_OnReceiveTrConditionEvent e)
        //{
        //    Console.WriteLine($"Api_OnReceiveTrCondition: ");
        //}

        private void Api_OnReceiveTrData(object sender, _DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            // GetResetEventForReqName
            Console.WriteLine($"Api_OnReceiveTrData: {e.sScrNo}, {e.sRQName}, {e.sTrCode}, {e.sRecordName}, {e.sPrevNext}");

            var result = GetReqResult(e.sRQName);
            // 요청명이 등록되지 않은 것은 무시한다.
            if (result == default)
                return;

            ///////////////////////
            // 주식기본정보 조회 //
            ///////////////////////
            if (e.sTrCode == "opt10001")
            {
                result.Value = new 주식기본정보
                {
                    종목코드 = Get<string>(0, "종목코드"),
                    종목명 = Get<string>(0, "종목명"),
                    결산월 = Get<int>(0, "결산월"),
                    액면가 = Get<float>(0, "액면가"),
                    자본금 = Get<decimal>(0, "자본금"),
                    상장주식 = Get<int>(0, "상장주식"),
                    신용비율 = Get<float>(0, "신용비율"),
                    연중최고 = Get<float>(0, "연중최고"),
                    연중최저 = Get<float>(0, "연중최저"),
                    시가총액 = Get<decimal>(0, "시가총액"),
                    외인소진률 = Get<float>(0, "외인소진률"),
                    대용가 = Get<decimal>(0, "대용가"),
                    PER = Get<float>(0, "PER"),
                    EPS = Get<decimal>(0, "EPS"),
                    ROE = Get<float>(0, "ROE"),
                    PBR = Get<float>(0, "PBR"),
                    EV = Get<float>(0, "EV"),
                    BPS = Get<decimal>(0, "BPS"),
                    매출액 = Get<decimal>(0, "매출액"),
                    영업이익 = Get<decimal>(0, "영업이익"),
                    당기순이익 = Get<decimal>(0, "당기순이익"),
                    N250최고 = Get<float>(0, "250최고"),
                    N250최저 = Get<float>(0, "250최저"),
                    시가 = Get<float>(0, "시가"),
                    고가 = Get<float>(0, "고가"),
                    저가 = Get<float>(0, "저가"),
                    상한가 = Get<float>(0, "상한가"),
                    하한가 = Get<float>(0, "하한가"),
                    기준가 = Get<float>(0, "기준가"),
                    예상체결가 = Get<float>(0, "예상체결가"),
                    예상체결수량 = Get<int>(0, "예상체결수량"),
                    N250최고가일 = Get<DateTime>(0, "250최고가일"),
                    N250최고가대비율 = Get<float>(0, "250최고가대비율"),
                    N250최저가일 = Get<DateTime>(0, "250최저가일"),
                    N250최저가대비율 = Get<float>(0, "250최저가대비율"),
                    현재가 = Get<float>(0, "현재가"),
                    대비기호 = Get<int>(0, "대비기호"),
                    전일대비 = Get<float>(0, "전일대비"),
                    등락율 = Get<float>(0, "등락율"),
                    거래량 = Get<int>(0, "거래량"),
                    거래대비 = Get<float>(0, "거래대비"),
                    액면가단위 = Get<string>(0, "액면가단위"),
                    유통주식 = Get<int>(0, "유통주식"),
                    유통비율 = Get<float>(0, "유통비율")
                };
            }
            ///////////////////////
            // 주식일봉정보 조회 //
            ///////////////////////
            else if (e.sTrCode == "opt10081")
            {
                var items = new List<주식일봉정보>();
                var count = api.GetDataCount(e.sRQName);
                var 종목코드 = "";
                for (var i = 0; i < count; i++)
                {
                    if (i == 0)
                        종목코드 = Get<string>(0, "종목코드");

                    var item = new 주식일봉정보
                    {
                        종목코드 = 종목코드,
                        현재가 = Get<float>(i, "현재가"),
                        거래량 = Get<int>(i, "거래량"),
                        거래대금 = Get<decimal>(i, "거래대금"),
                        일자 = Get<DateTime>(i, "일자"),
                        시가 = Get<float>(i, "시가"),
                        고가 = Get<float>(i, "고가"),
                        저가 = Get<float>(i, "저가")
                    };
                    items.Add(item);
                }
                result.Value = items;
                result.IsContinuous = e.sPrevNext == "2";
            }
            ///////////////////////
            // 주식분봉정보 조회 //
            ///////////////////////
            else if (e.sTrCode == "opt10080")
            {
                var items = new List<주식분봉정보>();
                var count = api.GetDataCount(e.sRQName);
                var 종목코드 = "";
                for (var i = 0; i < count; i++)
                {
                    if (i == 0)
                        종목코드 = Get<string>(0, "종목코드");

                    var item = new 주식분봉정보
                    {
                        종목코드 = 종목코드,
                        현재가 = Get<float>(i, "현재가").Abs(),
                        거래량 = Get<int>(i, "거래량"),
                        체결시간 = Get<DateTime>(i, "체결시간"),
                        시가 = Get<float>(i, "시가").Abs(),
                        고가 = Get<float>(i, "고가").Abs(),
                        저가 = Get<float>(i, "저가").Abs()
                    };
                    items.Add(item);
                }
                result.Value = items;
                result.IsContinuous = e.sPrevNext == "2";
            }
            /////////////////////////
            // 주식 매수/매도 주문 //
            /////////////////////////
            else if (e.sTrCode == "KOA_NORMAL_BUY_KP_ORD" || e.sTrCode == "KOA_NORMAL_SELL_KP_ORD")
            {
                var orderNo = Get<int>(0, "주문번호");
                result.Value = orderNo;
            }

            result.ResponseResetEvent.Set();
            
            ////
            TResult Get<TResult>(int index, string itemName) => api.GetCommData(e.sTrCode, e.sRQName, index, itemName).CastTo<TResult>();
        }

        private StockProviderException GetException(int errorCode)
        {
            var kind = errorCode switch
            {
                -200 => StockProviderExceptionKind.조회오류_과부하,
                -202 or -300 => StockProviderExceptionKind.조회오류_인자오류,
                -301 => StockProviderExceptionKind.주문오류_비밀번호불일치,
                _ => StockProviderExceptionKind.미정의오류
            };

            return new StockProviderException(kind, errorCode);
        }

        private void Api_OnReceiveRealData(object sender, _DKHOpenAPIEvents_OnReceiveRealDataEvent e)
        {
            Console.WriteLine($"Api_OnReceiveRealData: ");
        }

        private void Api_OnReceiveChejanData(object sender, _DKHOpenAPIEvents_OnReceiveChejanDataEvent e)
        {
            Console.WriteLine($"Api_OnReceiveChejanData: {e.sGubun}, {e.nItemCnt}, {e.sFIdList}");

            if (e.nItemCnt == 0)
                return;

            var fids = e.sFIdList.Split(';');
            foreach (var sFid in fids)
            {
                var fid = int.Parse(sFid);
                var fidResult = api.GetChejanData(fid);
                Console.WriteLine($"{fid}: {fidResult}");
            }
        }

        //private void Api_OnReceiveInvestRealData(object sender, _DKHOpenAPIEvents_OnReceiveInvestRealDataEvent e)
        //{
        //    Console.WriteLine($"Api_OnReceiveInvestRealData: ");
        //}

        //private void Api_OnReceiveRealCondition(object sender, _DKHOpenAPIEvents_OnReceiveRealConditionEvent e)
        //{
        //    Console.WriteLine($"Api_OnReceiveRealCondition: ");
        //}

        //// 스레드가 종료될 경우 호출되지 않는다.
        //protected override void OnFormClosed(FormClosedEventArgs e)
        //{
        //    api.CommTerminate();

        //    base.OnFormClosed(e);
        //}
        //// 스레드가 종료될 경우 호출되지 않는다.
        //protected override void OnClosed(EventArgs e)
        //{
        //    api.CommTerminate();

        //    base.OnClosed(e);
        //}

        private string GenerateReqName()
        {
            reqNum++;
            return $"Req{reqNum}";
        }

        private void Api_OnReceiveMsg(object sender, _DKHOpenAPIEvents_OnReceiveMsgEvent e)
        {
            Console.WriteLine($"Api_OnReceiveMsg: {e.sScrNo}, {e.sRQName}, {e.sTrCode}, {e.sMsg}");
        }

        private void Api_OnEventConnect(object sender, _DKHOpenAPIEvents_OnEventConnectEvent e)
        {
            //Console.WriteLine($"Api_OnEventConnect: {e.nErrCode}");

            try
            {
                var errorNo = e.nErrCode;
                if (errorNo == -100)
                {
                    lastException = new StockProviderLoginException(StockProviderExceptionKind.로그인오류_정보불일치);
                    return;
                }
                else if (errorNo == -101)
                {
                    lastException = new StockProviderLoginException(StockProviderExceptionKind.로그인오류_통신장애);
                    return;
                }
                else if (errorNo == -102)
                {
                    lastException = new StockProviderLoginException(StockProviderExceptionKind.로그인오류_버전처리);
                    return;
                }

                var temp = api.GetLoginInfo("ACCOUNT_CNT");
                var 보유계좌수 = int.Parse(temp);
                temp = api.GetLoginInfo("ACCLIST");
                var 보유계좌목록 = temp.Split(';');
                temp = api.GetLoginInfo("USER_ID");
                var 사용자ID = temp;
                temp = api.GetLoginInfo("USER_NAME");
                var 사용자명 = temp;
                temp = api.GetLoginInfo("GetServerGubun");
                var 서버구분 = temp == "1" ? 접속서버구분.모의투자서버 : 접속서버구분.실거래서버;

                사용자정보 = new(증권사구분.키움증권, 사용자ID, 사용자명, 보유계좌수, 보유계좌목록, 서버구분);
            }
            finally
            {
                waitConnectResetEvent.Set();
            }
        }

        private async Task<TResult> RequestData<TResult>(string trCode, params (string Key, string Value)[] @params)
            where TResult : class
        {
            TResult result = default;

            await Task.Run(() =>
            {
                // 요청 대기시간 0.2초 이내에 다시 요청을 하면 대기 후 요청한다.
                var interval = stopwatch.Elapsed - lastRequestTimeSpan;
                //Console.WriteLine($"!!!! {interval}, {RequestTimeInterval}");
                if (interval < RequestTimeInterval)
                    Thread.Sleep(RequestTimeInterval - interval);

                var nResult = 0;
                var reqName = GenerateReqName();
                api.Invoke(new Action(() =>
                {
                    foreach (var kv in @params)
                        api.SetInputValue(kv.Key, kv.Value);

                    nResult = api.CommRqData(reqName, trCode, 0, "101");
                }));
                if (nResult < 0)
                    throw GetException(nResult);

                (result, _) = WaitForReqResult<TResult>(reqName);

                RemoveReqResult(reqName);

                lastRequestTimeSpan = stopwatch.Elapsed;
            });

            return result;
        }

        private async Task<IReadOnlyList<TResult>> RequestMultiData<TResult>(string trCode, params (string Key, string Value)[] @params)
        {
            List<TResult> result = new List<TResult>();

            await Task.Run(() =>
            {
                var nResult = 0;
                var reqName = GenerateReqName();

                var isContinuous = false;
                while (true)
                {
                    // 요청 대기시간 0.2초 이내에 다시 요청을 하면 대기 후 요청한다.
                    var interval = stopwatch.Elapsed - lastRequestTimeSpan;
                    //Console.WriteLine($"!!!! {interval}, {RequestTimeInterval}");
                    if (interval < RequestTimeInterval)
                        Thread.Sleep(RequestTimeInterval - interval);

                    api.Invoke(new Action(() =>
                    {
                        foreach (var kv in @params)
                            api.SetInputValue(kv.Key, kv.Value);

                        nResult = api.CommRqData(reqName, trCode, isContinuous == true ? 2 : 0, "101");
                    }));
                    if (nResult < 0)
                        throw GetException(nResult);

                    List<TResult> block;
                    (block, isContinuous) = WaitForReqResult<List<TResult>>(reqName);
                    result.AddRange(block);

                    lastRequestTimeSpan = stopwatch.Elapsed;

                    if (isContinuous == false)
                        break;

                    GetReqResult(reqName)?.ResponseResetEvent.Reset();
                }

                RemoveReqResult(reqName);
            });

            return result;
        }

        // string 계좌번호, string 종목코드, 주문유형 주문유형, float 수량, float 가격, 거래구분 거래구분
        private async Task<주식주문정보> RequestOrder(string accountNo, string itemCode, int orderType, float quantity, float price, string orignOrderNo)
        {
            주식주문정보 result = default;

            await Task.Run(() =>
            {
                // 요청 대기시간 0.2초 이내에 다시 요청을 하면 대기 후 요청한다.
                var interval = stopwatch.Elapsed - lastOrderTimeSpan;
                //Console.WriteLine($"!!!! {interval}, {RequestTimeInterval}");
                if (interval < RequestTimeInterval)
                    Thread.Sleep(RequestTimeInterval - interval);

                var nResult = 0;
                var reqName = GenerateReqName();
                api.Invoke(new Action(() =>
                {
                    var nQuantity = (int)quantity;
                    var nPrice = (int)price;
                    // "03"은 시장가, "01"은 지정가. 주문가격이 0일 경우 시장가로 거래한다.
                    nResult = api.SendOrder(reqName, "101", accountNo, orderType, itemCode, nQuantity, nPrice, nPrice == 0 ? "03" : "00", orignOrderNo);
                }));
                if (nResult < 0)
                    throw GetException(nResult);

                (result, _) = WaitForReqResult<주식주문정보>(reqName);

                RemoveReqResult(reqName);

                lastOrderTimeSpan = stopwatch.Elapsed;
            });

            return result;
        }

        private (TResult, bool) WaitForReqResult<TResult>(string reqName)
            where TResult : class
        {
            var result = new ReqResult();
            reqNameToResultMap[reqName] = result;
            result.ResponseResetEvent.Wait();
            return (result.Value as TResult, result.IsContinuous);
        }

        private ReqResult GetReqResult(string reqName)
        {
            var bResult = reqNameToResultMap.TryGetValue(reqName, out var result);
            if (bResult == false)
                return default;

            return result;
        }

        private void RemoveReqResult(string reqName)
        {
            reqNameToResultMap.TryRemove(reqName, out _);
        }

        private class ReqResult
        {
            public object Value { get; set; }
            public bool IsContinuous { get; set; }
            public ManualResetEventSlim ResponseResetEvent { get; } = new ManualResetEventSlim(false);
        }

        public bool 연결_유무()
        {
            var result = 0;
            api.Invoke(new Action(() => result = api.GetConnectState()));
            if (result == 1)
                return true;

            return false;
        }

        public Task<주식기본정보> 주식_기본정보_조회(string 종목코드)
        {
            return RequestData<주식기본정보>("opt10001",
                ("종목코드", 종목코드)
            );
        }

        public Task<IReadOnlyList<주식일봉정보>> 주식_일봉정보_조회(string 종목코드, DateTime 기준일자)
        {
            return RequestMultiData<주식일봉정보>("opt10081",
                ("종목코드", 종목코드),
                ("기준일자", 기준일자.ToCombineDate()),
                ("수정주가구분", "1")
            );
        }

        public Task<IReadOnlyList<주식분봉정보>> 주식_분봉_조회(string 종목코드, 분봉구분 분봉구분)
        {
            return RequestMultiData<주식분봉정보>("opt10080",
                ("종목코드", 종목코드),
                ("틱범위", 분봉구분 switch
                {
                    분봉구분.분봉_1분봉 => "1",
                    분봉구분.분봉_5분봉 => "5",
                    분봉구분.분봉_10분봉 => "10",
                    분봉구분.분봉_15분봉 => "15",
                    분봉구분.분봉_30분봉 => "30",
                    분봉구분.분봉_45분봉 => "45",
                    분봉구분.분봉_60분봉 => "60",
                    _ => "30"
                }),
                ("수정주가구분", "1")
            );
        }

        public Task<주식주문정보> 주식_주문(string 계좌번호, string 종목코드, 주문유형 주문유형, float 수량, float 가격, 거래구분 거래구분)
        {
            // 시장가 거래의 경우 가격은 0원이어야 한다.
            if (거래구분 == 거래구분.시장가)
                가격 = 0;

            return RequestOrder(계좌번호, 종목코드,
                주문유형 switch
                {
                    주문유형.매수 => 1,
                    주문유형.매도 => 2,
                    _ => 1
                },
                수량, 가격,
                ""/*원주문번호*/);
        }

        public Task<주식주문정보> 주식_정정주문(주식주문정보 원주문, float 수량, float 가격)
        {
            return Task.FromResult(원주문);
        }

        public Task<bool> 주식_주문취소(주식주문정보 원주문)
        {
            return Task.FromResult(false);
        }
    }
}
