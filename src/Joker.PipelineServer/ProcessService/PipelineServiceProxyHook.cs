using System;
using System.Reflection;
using Castle.DynamicProxy;

namespace Joker.MultiProc.PipelineServer.ProcessService
{
    /// <summary>
    /// 管道服务代理挂钩
    /// </summary>
    [Serializable]
    internal class PipelineServiceProxyHook : IProxyGenerationHook
    {
        public void MethodsInspected()
        {
        }

        public void NonProxyableMemberNotification(Type type, MemberInfo memberInfo)
        {
            if (memberInfo.MemberType != MemberTypes.Method)
                return;
            var methodInfo = (MethodInfo) memberInfo;
            if (!methodInfo.IsSpecialName && methodInfo.IsPublic &&
                methodInfo.GetBaseDefinition().DeclaringType != typeof(object))
            {
                throw new PipelineMethodNotVirtualException(type, memberInfo);
            }
        }

        public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
        {
            if (!methodInfo.IsPipelineMethod() && !methodInfo.IsAllowExtendMethod())
                return false;
            PipelineServiceProxyHook.VerifyParams(type, methodInfo, methodInfo.GetParameters());
            return true;
        }

        internal static void VerifyParams(Type targetType, MethodInfo methodInfo, ParameterInfo[] parameters)
        {
            foreach (ParameterInfo parameter in parameters)
            {
                if (parameter.IsOut || parameter.ParameterType.IsByRef)
                    throw new PipelineMethodSignatureException($@"服务方法的参数不支持out, ref，当前方法名：{methodInfo.DeclaringType}.{methodInfo.Name}");
            }
            if (parameters.Length > 10)
                throw new PipelineMethodSignatureException($@"服务方法的参数个数必须小于等于10，当前方法名：{methodInfo.DeclaringType}.{methodInfo.Name}");
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (this == obj)
                return true;
            return obj.GetType() == this.GetType();
        }

        public override int GetHashCode()
        {
            return this.GetType().GetHashCode();
        }
    }
}
