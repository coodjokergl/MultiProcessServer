using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Joker.MultiProc.PipelineServer.ProcessService;

namespace Joker.MultiProc.Demo
{
    class Program
    {

        static ProcessService<ICalculator> Calc = new ProcessService<ICalculator>();

        static object _lockObj = new object();
        static void Main(string[] args)
        {
            try
            {
                SyncTest();//同步测试服务
                AsyncTest();//异步测试
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            Console.ReadKey();
        }

        /// <summary>
        /// 同步测试
        /// </summary>
        static void SyncTest()
        {
            int total = 200000;
            for (int i = 0; i < total; i++)
            {
                CalcImpl();
            }
            Console.WriteLine("调用已结束！");
        }

        /// <summary>
        /// 异步测试
        /// </summary>
        static void AsyncTest()
        {
            int total = 200000;
            for (int i = 0; i < total; i++)
            {
                CalcTask();
            }
        }

        private static readonly Random Rander = new Random();
        public static readonly List<string> _info = new List<string>();
        static async void CalcTask()
        {
            await Task.Run(CalcImpl);
        }

        static void CalcImpl()
        {

            try
            {
                var a = Rander.Next(1, 100);
                var b = Rander.Next(1, 100);

                var result = Calc.Instance.Sum(a, b);
                var msg = $@"{a}+{b} = {result}";

                lock (_lockObj)
                {
                    _info.Add(msg);
                    Console.WriteLine($@"第{_info.Count}次计算：");
                    Console.WriteLine(msg);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}
