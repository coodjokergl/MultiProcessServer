using System;
using Castle.DynamicProxy;

namespace Joker.MultiProc.PipelineServer.ProcessService
{
    /// <summary>
    /// 调用工厂
    /// </summary>
    public class ProcessServiceFactory
    {
        private  static readonly ProxyGenerationOptions Options =  new ProxyGenerationOptions((IProxyGenerationHook) new PipelineServiceProxyHook());
        

        internal static object GetServiceInstance(Type serviceType)
        {
            return Activator.CreateInstance(serviceType);
        }

        /// <summary>
        /// 生成客户端代理类
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static object CreateClientProxy(Type type)
        {
            var interceptor = new ClientInterceptor();

            return new ProxyGenerator().CreateInterfaceProxyWithoutTarget(type, ProcessServiceFactory.Options, new []
            {
                (IInterceptor) interceptor
            });
        }

        /// <summary>
        /// 生成客户端代理类
        /// </summary>
        /// <returns></returns>
        internal static IProcessInvoker CreateServiceProxy()
        {
            return new ServiceInvoker();
        }
    }
}
