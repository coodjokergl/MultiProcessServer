namespace Joker.MultiProc.PipelineServer.ServerLog
{
    internal class Logger
    {
        /// <summary>
        /// 日志单例
        /// </summary>
        public static log4net.ILog Log { get; } = log4net.LogManager.GetLogger(@"MultiProc.PipelineServer");
    }
}
