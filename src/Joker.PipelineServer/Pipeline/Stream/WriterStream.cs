using System;
using System.IO;

namespace Joker.MultiProc.PipelineServer.Pipeline.Stream
{
    /// <summary>
    /// 入参数据流
    /// </summary>
    [Serializable]
    internal class WriterStream
    {
        private readonly StreamWriter _ioStream;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ioStream"></param>
        public WriterStream(System.IO.Stream ioStream)
        {
            this._ioStream = new StreamWriter(ioStream);
            _ioStream.AutoFlush = true;
        }

        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public void Write(object result)
        {
            var data = JsonStreamSerializerResolver.Serializer.SerializeObject(result);
            _ioStream.WriteLine(data);
        }
    }
}
