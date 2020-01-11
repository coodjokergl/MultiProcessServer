namespace Joker.MultiProc.PipelineServer.ProcessService
{
    /// <summary>
    /// 表示需要延迟创建的对象包装类。
    /// </summary>
    /// <typeparam name="T">需要延迟创建的对象类型。</typeparam>
    public sealed class ProcessService<T> where T : IProcessService
    {
        private T _instance;
		
        /// <summary>
        /// 获取延迟创建的对象引用。
        /// </summary>
        public T Instance 
        {
            get
            {
                if (object.Equals(_instance, default(T)))
                {

                    if (ProcessEnvironment.IsServer)
                    {
                        _instance = (T)ProcessServiceFactory.GetServiceInstance(typeof(T));

                    }
                    else
                    {
                        _instance = (T)ProcessServiceFactory.CreateClientProxy(typeof(T));
                    }
                }
                return _instance;
            }
        }

    }
}
