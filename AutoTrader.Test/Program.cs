using AutoTrader.Core.StockProvider;
using AutoTrader.StockProvider.Kiwoom;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AutoTrader.Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var manager = new SessionManager();

            Console.WriteLine(manager.연결_유무());

            var userInfo = await manager.로그인();
            Console.WriteLine(userInfo);

            Console.WriteLine("Login!!");

            Console.WriteLine(manager.연결_유무());

            //for (var i = 0; i < 1000; i++)
            //{
            //    var result = await manager.주식_기본정보_조회("000020");
            //    Console.WriteLine(result);
            //}
            //var result = await manager.주식_일봉정보_조회("000020", DateTime.Now);
            //foreach (var item in result)
            //{
            //    Console.WriteLine(item);
            //}
            //var result = await manager.주식_분봉_조회("000020", 분봉구분.분봉_1분봉);
            //Console.WriteLine(result.Count);


            // 주문 확인
            Console.ReadLine();
            var 계좌번호 = userInfo.보유계좌목록.First();
            var 주문1 = await manager.주식_주문(계좌번호, "000020", 주문유형.매수, 0, 10, 거래구분.시장가);
            Console.WriteLine(주문1);
            //var 정정주문 = await manager.주식_정정주문(주문1, 0, 5);
            //Console.WriteLine(정정주문);
            var 취소주문 = await manager.주식_주문취소(주문1);
            Console.WriteLine(취소주문);

            Console.ReadLine();
        }
    }
}
