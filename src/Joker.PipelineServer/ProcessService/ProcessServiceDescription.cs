using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Joker.MultiProc.PipelineServer.ProcessService
{
    /// <summary>
    /// 远程服务描述
    /// </summary>
    internal class ProcessServiceDescription
    {
        internal readonly Type Type;
        private readonly Dictionary<MethodInfo, Lazy<IMethodDescription>> _methods;

        public ProcessServiceDescription(Type type)
        {
            this.Type = type;
            this._methods = this.LoadMethodDescriptionDic();
        }

        /// <summary>
        /// 加载方法描述
        /// </summary>
        /// <returns></returns>
        private Dictionary<MethodInfo, Lazy<IMethodDescription>> LoadMethodDescriptionDic()
        {
            var dictionary = new Dictionary<MethodInfo, Lazy<IMethodDescription>>();
            foreach (MethodInfo method in this.Type.GetMethods(BindingFlags.Instance | BindingFlags.Public))
            {
                MethodInfo item = method;
                dictionary.Add(item, new Lazy<IMethodDescription>(( Func<IMethodDescription> )( () => this.LoadMethodDescription(item) ), LazyThreadSafetyMode.ExecutionAndPublication));
            }
            return dictionary;
        }

        /// <summary>
        /// 加载方法描述
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        private IMethodDescription LoadMethodDescription(MethodInfo methodInfo)
        {
            var customAttribute = methodInfo.DeclaringType?.GetCustomAttribute<MultiProcessAttribute>();
            if (customAttribute == null) return null;
            return ( IMethodDescription )new LocalMethodDescription(methodInfo, customAttribute);
        }

        /// <summary>
        /// 获取方法描述
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        public IMethodDescription Get(MethodInfo methodInfo)
        {
            Lazy<IMethodDescription> lazy;
            if (this._methods.TryGetValue(methodInfo, out lazy))
            {
                return lazy.Value;
            } else
            {
                throw new InvalidProgramException($@"RemoteService初始化失败，无法找到方法缓存{Environment.NewLine}{string.Join(Environment.NewLine, new[]
                {
                    $@"类型名：{methodInfo.DeclaringType?.FullName}",
                    $@"方法名：{methodInfo.Name}"
                })}");
            }
        }

        /// <summary>
        /// 获取方法描述
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        public IMethodDescription Get(string methodInfo)
        {
            foreach (var method in _methods)
            {
                if(method.Key.Name != methodInfo) continue;
                return method.Value.Value;
            }
            
            throw new InvalidProgramException($@"RemoteService初始化失败，无法找到方法缓存{Environment.NewLine}{string.Join(Environment.NewLine, new[]
            {
                $@"类型名：{Type.DeclaringType?.FullName}",
                $@"方法名：{methodInfo}"
            })}");
        }
    }
}
