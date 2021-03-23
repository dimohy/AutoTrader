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

            Console.WriteLine($"계좌번호: {userInfo.보유계좌목록.First()}");

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
            //Console.ReadLine();
            var 계좌번호 = userInfo.보유계좌목록.First();
            //var 주문1 = await manager.주식_주문(계좌번호, "000020", 주문유형.매수, 0, 10, 거래구분.시장가);
            //Console.WriteLine(주문1);
            ////var 정정주문 = await manager.주식_정정주문(주문1, 0, 5);
            ////Console.WriteLine(정정주문);
            //var 취소주문 = await manager.주식_주문취소(주문1);
            //Console.WriteLine(취소주문);

            // 삼성전자(005930), LG전자(066570), SK하이닉스(000660), 현대차(005380), 기아차(000270)

            var accountNumber = userInfo.보유계좌목록.First();
            Console.WriteLine(accountNumber);

            // 계좌번호 : 8159796211

            var result = await manager.주식_계좌정보_조회("8159796211");
            Console.WriteLine(result);

            //manager.주식_실시간시세_호출 = x =>
            //{
            //    Console.WriteLine(x);
            //};
            //await manager.주식_실시간시세_등록("005380");

            //await manager.주식_주문("8159796211", "000020", 주문유형.매도, 0, 230, 거래구분.시장가);

            //await manager.주식_주문(계좌번호, "000270", 주문유형.매수, 0, 20, 거래구분.시장가);
           
            //await manager.주식_주문(계좌번호, "000270", 주문유형.매수, 82000, 10, 거래구분.지정가);



            Console.WriteLine("Press Enter To Exit.");
            Console.ReadLine();
        }
    }
}
