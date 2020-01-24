using System;
using System.Collections.Concurrent;

namespace Joker.MultiProc.PipelineServer.ProcessService
{
    /// <summary>
    /// 服务容器
    /// </summary>
    internal class ProcessServiceContainer
    {
        private readonly ConcurrentDictionary<Type, ProcessServiceDescription> _cache = new ConcurrentDictionary<Type, ProcessServiceDescription>();

        internal static ProcessServiceContainer Instance { get; } = new ProcessServiceContainer();

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Register<T>() where T : IProcessService
        {
            Register(typeof(T));
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        public void Register(Type type)
        {
            if(!type.IsPipeServer())
                throw new Exception($@"{type}不是有效的PipeServer");

            this._cache[type] = this.CreateServiceDescription(type);
        }

        public ProcessServiceDescription GetProcessServiceDescription(Type type)
        {
            if (this._cache.TryGetValue(type, out var serviceDescription))
                return serviceDescription;
            throw new InvalidOperationException("ProcessService必须先注册，才能使用！");
        }

        public ProcessServiceDescription GetProcessServiceDescription(string typeFullName)
        {
            foreach (var processServiceDescription in _cache)
            {
                if (processServiceDescription.Key.FullName != typeFullName) continue;
                return processServiceDescription.Value;
            }
            throw new InvalidOperationException("ProcessService必须先注册，才能使用！");
        }

        private ProcessServiceDescription CreateServiceDescription(Type type)
        {
            return new ProcessServiceDescription(type);
        }
    }

}
