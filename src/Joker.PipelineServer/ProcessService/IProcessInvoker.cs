using Joker.MultiProc.PipelineServer.Pipeline;

namespace Joker.MultiProc.PipelineServer.ProcessService
{
    /// <summary>
    /// 服务方法
    /// </summary>
    internal interface IProcessInvoker
    {
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        object Execute(CmdParam param);
    }
}
