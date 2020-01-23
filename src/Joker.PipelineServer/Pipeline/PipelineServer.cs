using System;
using System.Diagnostics;
using System.IO.Pipes;
using Joker.MultiProc.PipelineServer.ProcessService;
using Joker.MultiProc.PipelineServer.ServerLog;

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
        public const int MaxNumberServerInstance = 20;

        /// <inheritdoc />
        protected override PipeStream CreatePipeStream()
        {
            return new NamedPipeServerStream(ServerName, PipeDirection.InOut, MaxNumberServerInstance);
        }

        /// <inheritdoc />
        public override async void NextPipeServerAsync()
        {
            if (_runnerTask == null) return;
            //异步开启服务，自动建立
            await PipelineServerPool.CreateServerPipeLineAsync(ServerName);
        }

        /// <inheritdoc />
        protected override void TaskRun()
        {
            var server = PipeStreamInstance as NamedPipeServerStream;
            if (server == null) throw new ArgumentNullException(nameof(PipeStreamInstance));

            try
            {
                server.WaitForConnection();
                Debugger.Log(1, "服务管道", $@"服务：{this}服务已被链接!");
                Logger.Log.Info($"服务：{this}服务已被链接!");

                //1、启动运行
                base.TaskRun();

                //2、读取参数
                var param = Read();

                //3、触发激活
                ProcessEnvironment.TriggerActive(true);

                Debugger.Log(1, "服务管道", $@"服务端{this}接收命令{nameof(param.CmdId)}:{param.CmdId} {nameof(param.TargetTypeName)}:{param.TargetTypeName}");

                Logger.Log.Info($@"服务端{this}接收命令{nameof(param.CmdId)}:{param.CmdId} {nameof(param.TargetTypeName)}:{param.TargetTypeName}");

                //4、生成代理服务
                var proxy = ProcessServiceFactory.CreateServiceProxy();

                try
                {
                    //5、执行服务方法
                    param.Result = proxy.Execute(param);
                }
                catch (Exception exception)
                {
                    //5、返回异常
                    param.Result = null;
                    param.ErrorException = new PipelineServerInvokerException(param, exception);

                    Logger.Log.Error($@"{this}命令执行失败。", param.ErrorException);
                }

                //参数不用传递，减少进程通讯数据包大小。
                param.Args = null;

                //6、返回处理结果
                Post(param);

                //触发激活
                ProcessEnvironment.TriggerActive(false);
            }
            catch (TimeoutException exception)
            {
                Debugger.Log(2, "服务管道", $@"{this}服务链接超时，视为丢失链接！");
                Logger.Log.Fatal($@"{this}服务链接超时，视为丢失链接！",exception);
            }
            catch (Exception exception)
            {
                Debugger.Log(2, "服务管道", $@"{this}服务异常！{exception}");
                Logger.Log.Fatal($@"{this}服务异常！", exception);
            }
            finally
            {
                Debugger.Log(1, "服务管道", $@"服务：{this}管道关闭！");
                Logger.Log.Info($@"服务：{this}管道关闭！");
                Dispose();
            }
        }
    }
}
