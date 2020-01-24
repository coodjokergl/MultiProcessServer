using Joker.MultiProc.PipelineServer.Pipeline;

namespace Joker.MultiProc.PipelineServer.ProcessService
{
    /// <summary>
    /// 服务执行
    /// </summary>
    internal class ServiceInvoker : IProcessInvoker
    {
        /// <inheritdoc />
        public object Execute(CmdParam param)
        {
            //获取注册的方法
            var service = ProcessServiceContainer.Instance.GetProcessServiceDescription(param.TargetTypeName);

            return service.Get(param.ServiceName).Call(param.Args);
        }
    }

}
