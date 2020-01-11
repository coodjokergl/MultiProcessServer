using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Joker.MultiProc.PipelineServer.ProcessService
{
    /// <summary>
    /// 当一个应用了PipelineMethod特性的方法不为虚方法的时候抛出该错误。
    /// </summary>
    [Serializable]
    public class PipelineMethodNotVirtualException : Exception
    {
        /// <summary>
        /// 初始化<see cref="PipelineMethodNotVirtualException"/>对象。
        /// </summary>
        public PipelineMethodNotVirtualException()
        {
        }

        /// <summary>
        /// 初始化<see cref="PipelineMethodNotVirtualException"/>对象。
        /// </summary>
        /// <param name="message">描述错误的消息。</param>
        public PipelineMethodNotVirtualException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 初始化<see cref="PipelineMethodNotVirtualException"/>对象。
        /// </summary>
        /// <param name="message">描述错误的消息。</param>
        /// <param name="inner">获取导致当前异常的 <see cref="Exception"/>实例。</param>
        public PipelineMethodNotVirtualException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// 初始化 <see cref="PipelineMethodNotVirtualException"/> 对象。
        /// </summary>
        /// <param name="type">方法所在的类型。</param>
        /// <param name="method">方法元数据。</param>
        public PipelineMethodNotVirtualException(Type type, MemberInfo method)
            : this(string.Format("{0}类型的方法{1}必须为虚方法。", type.FullName, method.Name))
        {
        }

        /// <summary>
        /// 初始化 <see cref="PipelineMethodNotVirtualException"/> 对象。
        /// </summary>
        /// <param name="info"><see cref="T:System.Runtime.Serialization.SerializationInfo" />，它存有有关所引发异常的序列化的对象数据。</param>
        /// <param name="context"><see cref="T:System.Runtime.Serialization.StreamingContext" />，它包含有关源或目标的上下文信息。</param>
        protected PipelineMethodNotVirtualException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
