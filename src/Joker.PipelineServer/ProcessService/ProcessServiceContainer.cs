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

        public ProcessServiceDescription GetRemoveServiceDescription(Type type)
        {
            if (this._cache.TryGetValue(type, out var serviceDescription))
                return serviceDescription;
            throw new InvalidOperationException("RemoteService必须先注册，才能使用！");
        }

        public ProcessServiceDescription GetRemoveServiceDescription(string typeFullName)
        {
            foreach (var remoteServiceDescription in _cache)
            {
                if (remoteServiceDescription.Key.FullName != typeFullName) continue;
                return remoteServiceDescription.Value;
            }
            throw new InvalidOperationException("RemoteService必须先注册，才能使用！");
        }

        private ProcessServiceDescription CreateServiceDescription(Type type)
        {
            return new ProcessServiceDescription(type);
        }
    }

}
