using System;
using System.IO;
using System.Text;

namespace Joker.MultiProc.PipelineServer.Pipeline.Stream
{
    /// <summary>
    /// 入参数据流
    /// </summary>
    [Serializable]
    internal sealed class ReaderStream
    {
        private readonly StreamReader _ioStream;
        private readonly UnicodeEncoding _streamEncoding;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ioStream"></param>
        public ReaderStream(System.IO.Stream ioStream)
        {
            this._ioStream = new StreamReader(ioStream);
            _streamEncoding = new UnicodeEncoding();
        }
       
        /// <summary>
        /// 读取流数据
        /// </summary>
        /// <returns></returns>
        public string ReadValue()
        {
            return _ioStream.ReadLine()??string.Empty;
        }

        /// <summary>
        /// 读取信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ReadInfo<T>()
        {
            var data = ReadValue();
            return (T) JsonStreamSerializerResolver.Serializer.DeserializeObject(data, typeof(T));
        }

        /// <summary>
        /// 读取信息
        /// </summary>
        /// <returns></returns>
        public byte[] ReadInfo()
        {
            var data = ReadValue();
            return _streamEncoding.GetBytes(data);
        }
    }
}
