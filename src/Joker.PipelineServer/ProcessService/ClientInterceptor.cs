using System;
using Castle.DynamicProxy;
using Joker.MultiProc.PipelineServer.Pipeline;
using Newtonsoft.Json;

namespace Joker.MultiProc.PipelineServer.ProcessService
{
    /// <summary>
    /// 拦截器
    /// </summary>
    internal class ClientInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            RegisterProcessService.Register();

            //代理所有服务，转发到服务中
            IMethodDescription methodDescription = ProcessServiceContainer.Instance.GetRemoveServiceDescription(invocation.Method.DeclaringType).Get(invocation.Method);

            //初始化服务，如果子进程未启动，则启动服务
            Init(methodDescription);

            //发送服务请求 
            using (var caller = new PipelineClient(methodDescription.ServiceName))
            {
                //todo:超时配置暂不用，默认一直等待
                var returnValue = caller.Call(methodDescription,-1,invocation.Arguments);

                if (returnValue != null && invocation.Method.ReturnType != typeof(void))
                {
                    invocation.ReturnValue =
                        JsonConvert.DeserializeObject(Convert.ToString(returnValue), invocation.Method.ReturnType);
                }
            }
        }


        public void Init(IMethodDescription description)
        {
            StartUp.StartUpServer(description.ServiceName,ProcessEnvironment.ServiceCount.HasValue ? ProcessEnvironment.ServiceCount.Value : description.ServiceCount);
        }
    }

}
