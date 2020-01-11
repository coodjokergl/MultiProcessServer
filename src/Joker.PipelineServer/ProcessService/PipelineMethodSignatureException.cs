using System;
using System.Runtime.Serialization;

namespace Joker.MultiProc.PipelineServer.ProcessService
{
    /// <summary>
    /// 服务方法签名错误。
    /// </summary>
    [Serializable]
    public class PipelineMethodSignatureException : Exception
    {
        /// <summary>
        /// 初始化<see cref="PipelineMethodSignatureException"/> 对象。
        /// </summary>
        /// <param name="message">描述错误的消息。</param>
        public PipelineMethodSignatureException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 初始化<see cref="PipelineMethodSignatureException"/> 对象。
        /// </summary>
        /// <param name="message">描述错误的消息。</param>
        /// <param name="innerException">获取导致当前异常的 <see cref="Exception"/>实例。</param>
        public PipelineMethodSignatureException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// 初始化<see cref="PipelineMethodSignatureException"/> 对象。
        /// </summary>
        /// <param name="info"><see cref="T:System.Runtime.Serialization.SerializationInfo" />，它存有有关所引发异常的序列化的对象数据。</param>
        /// <param name="context"><see cref="T:System.Runtime.Serialization.StreamingContext" />，它包含有关源或目标的上下文信息。</param>
        protected PipelineMethodSignatureException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
