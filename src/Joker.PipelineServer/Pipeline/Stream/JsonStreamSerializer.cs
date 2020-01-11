using System;
using Newtonsoft.Json;

namespace Joker.MultiProc.PipelineServer.Pipeline.Stream
{
    /// <summary>
    /// Json格式序列化
    /// </summary>
    internal class JsonStreamSerializer:IStreamSerializer
    {
        /// <inheritdoc />
        public string SerializeObject(object obj)
        {
            if (obj == null) return string.Empty;
            return JsonConvert.SerializeObject(obj);
        }

        /// <inheritdoc />
        public object DeserializeObject(string obj, Type type)
        {
            if (string.IsNullOrEmpty(obj)) return null;
            return  JsonConvert.DeserializeObject(obj,type);
        }
    }
}
