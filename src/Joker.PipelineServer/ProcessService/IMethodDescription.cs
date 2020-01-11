namespace Joker.MultiProc.PipelineServer.ProcessService
{
    /// <summary>
    /// 服务方法描述接口
    /// </summary>
    internal interface IMethodDescription
    {
        /// <summary>
        /// 方法调用
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        object Call(object[] args);

        /// <summary>
        /// 服务名称
        /// </summary>
        string ServiceName { get; }

        /// <summary>
        /// 服务数量
        /// </summary>
        int ServiceCount { get; }

        /// <summary>
        /// 类型名称
        /// </summary>
        string TypeName { get; }

        /// <summary>
        /// 方法名称
        /// </summary>
        string MethodName { get; }
    }

}
