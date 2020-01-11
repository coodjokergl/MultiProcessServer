using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using Joker.MultiProc.PipelineServer.Pipeline.Stream;

namespace Joker.MultiProc.PipelineServer.Pipeline
{
    /// <summary>
    /// 管道服务基类
    /// </summary>
    internal abstract class PipelineBase : IDisposable
    {
        /// <summary>
        /// 管道ID。唯一标识一个管道服务，没什么具体含义，调试用的
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServerName { get; protected set; }

        /// <summary>
        /// 管道数据流
        /// </summary>
        protected PipeStream PipeStreamInstance { get; private set; }


        private AutoResetEvent _serverAutoRun = new AutoResetEvent(false);

        #region 数据流的读写器

        protected WriterStream Writer { get; private set; }
        protected ReaderStream Reader { get; private set; }

        #endregion

        #region 管线任务

        protected Task _runnerTask;

        #endregion


        protected PipelineBase()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// 开启数据流
        /// </summary>
        /// <returns></returns>
        protected abstract PipeStream CreatePipeStream();

        /// <summary>
        /// 下一次服务实例。
        /// 当管道被关闭后，自动启动一个新的带连接资源
        /// </summary>
        public abstract void NextPipeServerAsync();

        /// <summary>
        /// 是否就绪
        /// </summary>
        public bool IsReady => PipeStreamInstance != null && PipeStreamInstance.IsConnected;

        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="serverName">服务名称</param>
        public void Start(string serverName)
        {
            if (ServerName != serverName || PipeStreamInstance == null)
            {
                ServerName = serverName;
                PipeStreamInstance = CreatePipeStream();
            }

            //启动
            TaskRun();
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="serverName">服务名称</param>
        public async void StartAsync(string serverName)
        {
            ServerName = serverName;
            PipeStreamInstance = CreatePipeStream();
            //启动
            _runnerTask = Task.Factory.StartNew(TaskRun);

            await _runnerTask;
        }


        protected virtual void TaskRun()
        {
            Writer = new WriterStream(PipeStreamInstance);
            Reader = new ReaderStream(PipeStreamInstance);
        }

        public virtual void Dispose()
        {
            if (PipeStreamInstance != null && PipeStreamInstance.IsConnected)
            {
                PipeStreamInstance.Close();
                PipeStreamInstance.Dispose();
                PipeStreamInstance = null;
            }

            var evt = _serverAutoRun;
            _serverAutoRun = null;

            if (evt != null)
            {
                evt.Close();
                evt.Dispose();
            }

            PipelineServerPool.DisposablePipeLineAsync(Id);
        }


        /// <summary>
        /// 发送数据。阻塞方式
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual bool Post(CmdParam data)
        {
            if (!IsReady)
            {
                Debugger.Log(1, "发送信息", $@"管道未就绪！");
                return false;
            }

            try
            {
                //发送数据
                Writer.Write(data);
                //等待对方进程接收
                PipeStreamInstance.WaitForPipeDrain();

                return true;
            }
            catch (Exception)
            {
                Close();//关闭
                throw;
            }
        }


        /// <summary>
        /// 读取数据。阻塞
        /// </summary>
        /// <returns></returns>
        protected virtual CmdParam Read()
        {
            if (!IsReady)
            {
                Debugger.Log(1, "读取信息", @"管道未就绪！");
                return null;
            }

            try
            {
                //读取信息 阻塞
                var readData = Reader.ReadInfo<CmdParam>();
                if(readData == null) return null;

                if (readData.ErrorException != null)
                {
                    throw readData.ErrorException;
                }
                return readData;
            }
            catch
            {
                Close();
                throw;
            }
        }

        protected virtual void Run()
        {
            _serverAutoRun.WaitOne();
        }

        protected virtual void Close()
        {
            _serverAutoRun.Set();
        }
    }
}
