using System;

namespace Joker.MultiProc.PipelineServer.ProcessService
{
    /// <summary>
    /// 多进程服务标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
    public class MultiProcessAttribute : Attribute
    {

        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// 服务实现类型
        /// </summary>
        public Type ServiceType { get; set; }

        /// <summary>
        /// 服务进程数量
        /// </summary>
        public int ServiceCount { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type">类型全称</param>
        /// <param name="serviceName">服务名称</param>
        /// <param name="serviceCount">进程数量</param>
        public MultiProcessAttribute(Type type,string serviceName,int serviceCount = 1)
        {
            this.ServiceType = type;
            this.ServiceName = serviceName;
            this.ServiceCount = serviceCount;
        }
    }

}
