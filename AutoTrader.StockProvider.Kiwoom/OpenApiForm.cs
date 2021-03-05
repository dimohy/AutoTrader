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
        private uint reqNum;
        private readonly ConcurrentDictionary<string, (ManualResetEventSlim, RefValue<object>)> reqNameToResultMap = new();
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
            api.OnEventConnect += Api_OnEventConnect;
            api.OnReceiveMsg += Api_OnReceiveMsg;
            api.OnReceiveRealCondition += Api_OnReceiveRealCondition;
            api.OnReceiveInvestRealData += Api_OnReceiveInvestRealData;
            api.OnReceiveChejanData += Api_OnReceiveChejanData;
            api.OnReceiveRealData += Api_OnReceiveRealData;
            api.OnReceiveTrData += Api_OnReceiveTrData;
            api.OnReceiveTrCondition += Api_OnReceiveTrCondition;
            api.OnReceiveConditionVer += Api_OnReceiveConditionVer;

            Controls.Add(api);

            base.OnLoad(e);

            InitCompleteEvent.Set();
        }

        private void Api_OnReceiveConditionVer(object sender, _DKHOpenAPIEvents_OnReceiveConditionVerEvent e)
        {
            Console.WriteLine($"Api_OnReceiveConditionVer: ");
        }

        private void Api_OnReceiveTrCondition(object sender, _DKHOpenAPIEvents_OnReceiveTrConditionEvent e)
        {
            Console.WriteLine($"Api_OnReceiveTrCondition: ");
        }

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
                result.Item2.Value = new 주식기본정보
                {
                    종목코드 = api.GetCommData(e.sTrCode, e.sRQName, 0, "종목코드").Trim(),
                    종목명 = api.GetCommData(e.sTrCode, e.sRQName, 0, "종목명").Trim(),
                    결산월 = api.GetCommData(e.sTrCode, e.sRQName, 0, "결산월").CastTo<int>(),
                    액면가 = api.GetCommData(e.sTrCode, e.sRQName, 0, "액면가").CastTo<float>(),
                    자본금 = api.GetCommData(e.sTrCode, e.sRQName, 0, "자본금").CastTo<decimal>(),
                    상장주식 = api.GetCommData(e.sTrCode, e.sRQName, 0, "상장주식").CastTo<int>(),
                    신용비율 = api.GetCommData(e.sTrCode, e.sRQName, 0, "신용비율").CastTo<float>(),
                    연중최고 = api.GetCommData(e.sTrCode, e.sRQName, 0, "연중최고").CastTo<float>(),
                    연중최저 = api.GetCommData(e.sTrCode, e.sRQName, 0, "연중최저").CastTo<float>(),
                    시가총액 = api.GetCommData(e.sTrCode, e.sRQName, 0, "시가총액").CastTo<decimal>(),
                    외인소진률 = api.GetCommData(e.sTrCode, e.sRQName, 0, "외인소진률").CastTo<float>(),
                    대용가 = api.GetCommData(e.sTrCode, e.sRQName, 0, "대용가").CastTo<decimal>(),
                    PER = api.GetCommData(e.sTrCode, e.sRQName, 0, "PER").CastTo<float>(),
                    EPS = api.GetCommData(e.sTrCode, e.sRQName, 0, "EPS").CastTo<decimal>(),
                    ROE = api.GetCommData(e.sTrCode, e.sRQName, 0, "ROE").CastTo<float>(),
                    PBR = api.GetCommData(e.sTrCode, e.sRQName, 0, "PBR").CastTo<float>(),
                    EV = api.GetCommData(e.sTrCode, e.sRQName, 0, "EV").CastTo<float>(),
                    BPS = api.GetCommData(e.sTrCode, e.sRQName, 0, "BPS").CastTo<decimal>(),
                    매출액 = api.GetCommData(e.sTrCode, e.sRQName, 0, "매출액").CastTo<decimal>(),
                    영업이익 = api.GetCommData(e.sTrCode, e.sRQName, 0, "영업이익").CastTo<decimal>(),
                    당기순이익 = api.GetCommData(e.sTrCode, e.sRQName, 0, "당기순이익").CastTo<decimal>(),
                    N250최고 = api.GetCommData(e.sTrCode, e.sRQName, 0, "250최고").CastTo<float>(),
                    N250최저 = api.GetCommData(e.sTrCode, e.sRQName, 0, "250최저").CastTo<float>(),
                    시가 = api.GetCommData(e.sTrCode, e.sRQName, 0, "시가").CastTo<float>(),
                    고가 = api.GetCommData(e.sTrCode, e.sRQName, 0, "고가").CastTo<float>(),
                    저가 = api.GetCommData(e.sTrCode, e.sRQName, 0, "저가").CastTo<float>(),
                    상한가 = api.GetCommData(e.sTrCode, e.sRQName, 0, "상한가").CastTo<float>(),
                    하한가 = api.GetCommData(e.sTrCode, e.sRQName, 0, "하한가").CastTo<float>(),
                    기준가 = api.GetCommData(e.sTrCode, e.sRQName, 0, "기준가").CastTo<float>(),
                    예상체결가 = api.GetCommData(e.sTrCode, e.sRQName, 0, "예상체결가").CastTo<float>(),
                    예상체결수량 = api.GetCommData(e.sTrCode, e.sRQName, 0, "예상체결수량").CastTo<int>(),
                    N250최고가일 = api.GetCommData(e.sTrCode, e.sRQName, 0, "250최고가일").CastTo<DateTime>(),
                    N250최고가대비율 = api.GetCommData(e.sTrCode, e.sRQName, 0, "250최고가대비율").CastTo<float>(),
                    N250최저가일 = api.GetCommData(e.sTrCode, e.sRQName, 0, "250최저가일").CastTo<DateTime>(),
                    N250최저가대비율 = api.GetCommData(e.sTrCode, e.sRQName, 0, "250최저가대비율").CastTo<float>(),
                    현재가 = api.GetCommData(e.sTrCode, e.sRQName, 0, "현재가").CastTo<float>(),
                    대비기호 = api.GetCommData(e.sTrCode, e.sRQName, 0, "대비기호").CastTo<int>(),
                    전일대비 = api.GetCommData(e.sTrCode, e.sRQName, 0, "전일대비").CastTo<float>(),
                    등락율 = api.GetCommData(e.sTrCode, e.sRQName, 0, "등락율").CastTo<float>(),
                    거래량 = api.GetCommData(e.sTrCode, e.sRQName, 0, "거래량").CastTo<int>(),
                    거래대비 = api.GetCommData(e.sTrCode, e.sRQName, 0, "거래대비").CastTo<float>(),
                    액면가단위 = api.GetCommData(e.sTrCode, e.sRQName, 0, "액면가단위").Trim(),
                    유통주식 = api.GetCommData(e.sTrCode, e.sRQName, 0, "유통주식").CastTo<int>(),
                    유통비율 = api.GetCommData(e.sTrCode, e.sRQName, 0, "유통비율").CastTo<float>()
                };
            }
            else if (e.sTrCode == "opt10081")
            {
                var items = new List<주식일봉정보>();
                var count = api.GetDataCount(e.sRQName);
                var 종목코드 = "";
                for (var i = 0; i < count; i++)
                {
                    if (i == 0)
                        종목코드 = api.GetCommData(e.sTrCode, e.sRQName, 0, "종목코드").Trim();

                    var item = new 주식일봉정보
                    {
                        종목코드 = 종목코드,
                        현재가 = api.GetCommData(e.sTrCode, e.sRQName, i, "현재가").CastTo<float>(),
                        거래량 = api.GetCommData(e.sTrCode, e.sRQName, i, "거래량").CastTo<int>(),
                        거래대금 = api.GetCommData(e.sTrCode, e.sRQName, i, "거래대금").CastTo<decimal>(),
                        일자 = api.GetCommData(e.sTrCode, e.sRQName, i, "일자").CastTo<DateTime>(),
                        시가 = api.GetCommData(e.sTrCode, e.sRQName, i, "시가").CastTo<float>(),
                        고가 = api.GetCommData(e.sTrCode, e.sRQName, i, "고가").CastTo<float>(),
                        저가 = api.GetCommData(e.sTrCode, e.sRQName, i, "저가").CastTo<float>()
                    };
                    items.Add(item);
                }
                Console.WriteLine(items.First());
                result.Item2.Value = items;
                result.Item2.IsContinuous = e.sPrevNext == "2";
            }
            else if (e.sTrCode == "opt10080")
            {
                var items = new List<주식분봉정보>();
                var count = api.GetDataCount(e.sRQName);
                var 종목코드 = "";
                for (var i = 0; i < count; i++)
                {
                    if (i == 0)
                        종목코드 = api.GetCommData(e.sTrCode, e.sRQName, 0, "종목코드").Trim();

                    var item = new 주식분봉정보
                    {
                        종목코드 = 종목코드,
                        현재가 = api.GetCommData(e.sTrCode, e.sRQName, i, "현재가").CastTo<float>().Abs(),
                        거래량 = api.GetCommData(e.sTrCode, e.sRQName, i, "거래량").CastTo<int>(),
                        체결시간 = api.GetCommData(e.sTrCode, e.sRQName, i, "체결시간").CastTo<DateTime>(),
                        시가 = api.GetCommData(e.sTrCode, e.sRQName, i, "시가").CastTo<float>().Abs(),
                        고가 = api.GetCommData(e.sTrCode, e.sRQName, i, "고가").CastTo<float>().Abs(),
                        저가 = api.GetCommData(e.sTrCode, e.sRQName, i, "저가").CastTo<float>().Abs()
                    };
                    items.Add(item);
                }
                result.Item2.Value = items;
                result.Item2.IsContinuous = e.sPrevNext == "2";
            }

            result.Item1.Set();


        }

        private StockProviderException GetException(int errorCode)
        {
            return errorCode switch
            {
                -200 => new StockProviderException(StockProviderExceptionKind.조회오류_과부하, errorCode),
                -202 => new StockProviderException(StockProviderExceptionKind.조회오류_인자오류, errorCode),
                -300 => new StockProviderException(StockProviderExceptionKind.조회오류_인자오류, errorCode),
                _ => new StockProviderException(StockProviderExceptionKind.미정의오류, errorCode)
            };
        }

        private void Api_OnReceiveRealData(object sender, _DKHOpenAPIEvents_OnReceiveRealDataEvent e)
        {
            Console.WriteLine($"Api_OnReceiveRealData: ");
        }

        private void Api_OnReceiveChejanData(object sender, _DKHOpenAPIEvents_OnReceiveChejanDataEvent e)
        {
            Console.WriteLine($"Api_OnReceiveChejanData: ");
        }

        private void Api_OnReceiveInvestRealData(object sender, _DKHOpenAPIEvents_OnReceiveInvestRealDataEvent e)
        {
            Console.WriteLine($"Api_OnReceiveInvestRealData: ");
        }

        private void Api_OnReceiveRealCondition(object sender, _DKHOpenAPIEvents_OnReceiveRealConditionEvent e)
        {
            Console.WriteLine($"Api_OnReceiveRealCondition: ");
        }

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
            Console.WriteLine($"Api_OnEventConnect: {e.nErrCode}");

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

                    GetReqResult(reqName).Item1.Reset();
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
                    var nResult = api.SendOrder(reqName, "101", accountNo, orderType, itemCode, nQuantity, nPrice, nPrice == 0 ? "03" : "00", orignOrderNo);
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
            (ManualResetEventSlim, RefValue<object>) result = (new(false), new());
            reqNameToResultMap[reqName] = result;
            result.Item1.Wait();
            return (result.Item2.Value as TResult, result.Item2.IsContinuous);
        }

        private (ManualResetEventSlim, RefValue<object>) GetReqResult(string reqName)
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

        private class RefValue<T>
        {
            public T Value { get; set; }
            public bool IsContinuous { get; set; }
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
