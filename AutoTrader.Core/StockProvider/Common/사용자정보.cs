using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTrader.Core.StockProvider.Common
{
    public record 사용자정보(증권사구분 증권사, string 사용자ID, string 사용자명, int 보유계좌수, IEnumerable<string> 보유계좌목록, 접속서버구분 접속서버);

    public enum 접속서버구분
    {
        실거래서버,
        모의투자서버
    }

    /// <summary>
    /// 증권사 구분
    /// 
    /// 증권사의 값을 변경하지 말 것
    /// </summary>
    public enum 증권사구분
    {
        키움증권 = 1,
        이베스트 = 2,
    }
}
