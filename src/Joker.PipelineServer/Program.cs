using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Joker.MultiProc.PipelineServer.ProcessService;
using Joker.MultiProc.PipelineServer.ServerLog;

namespace Joker.MultiProc.PipelineServer
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

            try
            {
                //启动服务
                StartUp.RunServer(args.FirstOrDefault());
            }
            catch (Exception exception)
            {
               Logger.Log.Fatal($@"服务启动失败！",exception);
               return;
            }

            //启动主进程循环
            while (!ProcessEnvironment.TryExist())
            {
                Thread.Sleep(10 * 1000);
            }
        }

        /// <summary>
        /// 未知异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Debugger.Log(3, "异常", e.ExceptionObject?.ToString());
            Logger.Log.Error("未捕获的线程异常", e.ExceptionObject as Exception);
        }
    }
}
