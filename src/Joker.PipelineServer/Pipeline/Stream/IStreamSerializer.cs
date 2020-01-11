using System;

namespace Joker.MultiProc.PipelineServer.Pipeline.Stream
{
    /// <summary>
    /// 数据流序列化
    /// </summary>
    public interface IStreamSerializer
    {
        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        string SerializeObject(object obj);

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <param name="obj">对象数据</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        object DeserializeObject(string obj,Type type);
    }
}
