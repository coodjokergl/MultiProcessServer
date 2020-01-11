using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Security.Principal;
using System.Threading;
using Joker.MultiProc.PipelineServer.ProcessService;

namespace Joker.MultiProc.PipelineServer.Pipeline
{
    /// <summary>
    /// 客户端管道
    /// </summary>
    internal class PipelineClient:PipelineBase
    {
        internal PipelineClient(string serverName)
        {
            ServerName = serverName;
        }

        /// <summary>
        /// 链接超时时间
        /// </summary>
        private const int ClientConnectTimeout = 10000;

        /// <inheritdoc />
        protected override PipeStream CreatePipeStream()
        {
            return new NamedPipeClientStream(".", ServerName, PipeDirection.InOut, PipeOptions.None,
                TokenImpersonationLevel.Impersonation);
        }

        /// <inheritdoc />
        protected override void TaskRun()
        {
            var client = PipeStreamInstance as NamedPipeClientStream;
            if (client == null) throw new Exception($@"{nameof(PipeStreamInstance)}类型错误！");

            try
            {
                //链接服务端 调试模式下不超时
                if (ProcessEnvironment.IsDebug)
                {
                    client.Connect();
                }
                else
                {
                    client.Connect(ClientConnectTimeout);
                }

                //基类服务
                base.TaskRun();

                //运行，阻止链接关闭
                Run();
            }
            catch (TimeoutException)
            {
                Debugger.Log(2,"客户端管道",$@"客户端：{ServerName}({Id})服务链接超时");
            }
            finally
            {
                Debugger.Log(1,"客户端管道",$"客户端：{ServerName}({Id})管道即将关闭");
                Dispose();
            }
        }

        /// <inheritdoc />
        public override void NextPipeServerAsync()
        {
            //客户端不用自动重新建立服务资源管道
        }

        /// <summary>
        /// 回调
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public object Call(IMethodDescription cmd,int timeOut,params object[] args)
        {
            //生成调用命令参数
            var cmdParam = new CmdParam()
            {
                CmdId = Guid.NewGuid(),
                TargetTypeName = cmd.TypeName,
                ServiceName = cmd.MethodName,
                Args = args
            };
            
            var callInvoker = (ThreadStart)(() =>
            {
                //发送调用请求
                Post(cmdParam);
                //读取执行结果
                var result = Read();
                cmdParam.Result = result?.Result;
            });

            _runAction = new Action(() =>
            {
                var gotEvent = new AutoResetEvent(false);
                var getEvent = new AutoResetEvent(false);
                if (Debugger.IsAttached || timeOut <= 0)
                {
                    callInvoker();
                }
                else
                {
                    var thread = new Thread(callInvoker);
                    thread.Start();
                    getEvent.Set();
                    if (!gotEvent.WaitOne(timeOut))
                    {
                        thread.Abort();
                        throw new TimeoutException($"客户端：{ServerName}({Id})。请求：{cmd}发生超时！");
                    }
                }
            });

            Start(ServerName);

            return cmdParam.Result;
        }

        private Action _runAction;

        /// <inheritdoc />
        protected override void Run()
        {
            _runAction?.Invoke();
        }
    }
}
