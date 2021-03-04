using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTrader.Core.StockProvider
{
    public static class StockProviderExceptions
    {
        private static Dictionary<StockProviderExceptionKind, string> exceptionMessageMap = new()
        {
            [StockProviderExceptionKind.미정의오류] = "정의되지 않은 오류입니다.",

            [StockProviderExceptionKind.로그인오류_통신장애] = "통신장애로 로그인이 실패하였습니다.",
            [StockProviderExceptionKind.로그인오류_정보불일치] = "로그인 정보가 일치하지 않습니다.",
            [StockProviderExceptionKind.로그인오류_타임아웃] = "로그인 입력 시간이 초과되었습니다.",
            [StockProviderExceptionKind.로그인오류_버전처리] = "버전 업데이트가 실패하였습니다.",

            [StockProviderExceptionKind.조회오류_인자오류] = "조회 인자가 올바르지 않습니다.",
            [StockProviderExceptionKind.조회오류_과부하] = "조회가 과도하여 실패하였습니다."
        };

        public static string GetExceptionMessage(StockProviderExceptionKind kind, int errorCode)
        {
            var bResult = exceptionMessageMap.TryGetValue(kind, out var result);
            if (bResult == false)
                return "정의되지 않은 오류입니다.";

            if (errorCode != 0)
                result = $"{result}({errorCode})";

            return result;
        }
    }

    public class StockProviderException : Exception
    {
        public StockProviderExceptionKind ExceptionKind { get; }

        public StockProviderException(StockProviderExceptionKind kind, int errorCode = 0, Exception innerException = default) : base(StockProviderExceptions.GetExceptionMessage(kind, errorCode), innerException)
        {
            this.ExceptionKind = kind;
        }
    }

    public class StockProviderLoginException : StockProviderException
    {
        public StockProviderLoginException(StockProviderExceptionKind kind, int errorCode = 0, Exception innerException = null) : base(kind, errorCode, innerException)
        {
        }
    }

    public enum StockProviderExceptionKind
    {
        미정의오류,
        로그인오류_통신장애,
        로그인오류_정보불일치,
        로그인오류_타임아웃,
        로그인오류_버전처리,

        조회오류_인자오류,
        조회오류_과부하
    }
}
