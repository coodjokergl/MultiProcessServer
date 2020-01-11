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

        static void Main(string[] args)
        {
            //var result = Calc.Instance.Get();
            //Console.WriteLine(result);

            //try
            //{
            //    Calc.Instance.Task();
            //}
            //catch (Exception exception)
            //{
            //    Console.WriteLine(exception);
            //}
            int total = 20;
            for (int i = 0; i < total; i++)
            {
                CalcTask();
            }

            while (true)
            {
                lock (_info)
                {
                    if (_info.Count > total - 5) break;

                    Console.WriteLine($@"{_info.Count} 完成！");
                }
               
                Thread.Sleep(1000);
            }
        }

        private static Random Rander = new Random();
        static List<string> _info = new List<string>();
        static async void CalcTask()
        {
            await Task.Run(() =>
            {
                try
                {
                    var a = Rander.Next(1, 100);
                    var b = Rander.Next(1, 100);

                    var result = Calc.Instance.Sum(a, b);
                    var msg = $@"{a}+{b} = {result}";
                    Console.WriteLine(msg);

                    lock (_info)
                    {
                        _info.Add(msg);
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            });
        }
    }
}
