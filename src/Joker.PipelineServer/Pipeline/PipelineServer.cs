using System;
using System.Diagnostics;
using System.IO.Pipes;
using Joker.MultiProc.PipelineServer.ProcessService;

namespace Joker.MultiProc.PipelineServer.Pipeline
{
    /// <summary>
    /// 服务管道
    /// </summary>
    internal class PipelineServer : PipelineBase
    {
        /// <summary>
        /// 管线最大并发线程数
        /// </summary>
        public const int MaxNumberServerInstance = 10;

        /// <inheritdoc />
        protected override PipeStream CreatePipeStream()
        {
            return new NamedPipeServerStream(ServerName, PipeDirection.InOut, MaxNumberServerInstance);
        }

        /// <inheritdoc />
        public override  void NextPipeServerAsync()
        {
            if(_runnerTask == null) return;
            //异步开启服务，自动建立
            //await PipelineServerPool.CreateServerPipeLineAsync(ServerName);
        }

        /// <inheritdoc />
        protected override void TaskRun()
        {
            var server = PipeStreamInstance as NamedPipeServerStream;
            if (server == null) throw new ArgumentNullException(nameof(PipeStreamInstance));

            try
            {
                server.WaitForConnection();
                Debugger.Log(1, "服务管道", $@"服务：{ServerName}({Id})服务已被链接!");

                base.TaskRun();

                Listen();
            }
            catch (TimeoutException)
            {
                Debugger.Log(2, "服务管道", $@"服务：{ServerName}({Id})服务链接超时，视为丢失链接");
            }
            catch (Exception exception)
            {
                Debugger.Log(2, "服务管道", $@"服务：{ServerName}({Id})服务异常。{exception}");
            }
            finally
            {
                Debugger.Log(1, "服务管道", $@"服务：{ServerName}({Id})管道关闭！");
                Dispose();
            }
        }

        /// <summary>
        /// 监听
        /// </summary>
        private void Listen()
        {
            while (true)
            {
                var param = Read();

                //触发激活
                ProcessEnvironment.TriggerActive(true);

                Debugger.Log(1, "服务管道", $@"服务端接收命令{nameof(param.CmdId)}:{param.CmdId} {nameof(param.TargetTypeName)}:{param.TargetTypeName} 自定义结果:{param.Result}");

                //生成代理服务
                var proxy = ProcessServiceFactory.CreateServiceProxy();

                try
                {
                    //执行服务方法
                    param.Result = proxy.Execute(param);
                }
                catch (Exception exception)
                {
                    //返回异常
                    param.Result = null;
                    param.ErrorException = new PipelineServerInvokerException(param,exception);
                }

                //参数不用传递了
                param.Args = null;

                //返回处理结果
                Post(param);

                //触发激活
                ProcessEnvironment.TriggerActive(false);
            }
        }

       
    }
}
