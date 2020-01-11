using System;
using System.Runtime.Serialization;

namespace Joker.MultiProc.PipelineServer.Pipeline
{
    /// <summary>
    /// 当子进程服务回调发送错误时抛出
    /// </summary>
    [Serializable]
    public class PipelineServerInvokerException : Exception
    {
        /// <summary>
        /// 初始化<see cref="PipelineServerInvokerException"/>对象。
        /// </summary>
        public PipelineServerInvokerException()
        {
        }

        /// <summary>
        /// 初始化<see cref="PipelineServerInvokerException"/>对象。
        /// </summary>
        /// <param name="message">描述错误的消息。</param>
        public PipelineServerInvokerException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 初始化<see cref="PipelineServerInvokerException"/>对象。
        /// </summary>
        /// <param name="message">描述错误的消息。</param>
        /// <param name="inner">获取导致当前异常的 <see cref="Exception"/>实例。</param>
        public PipelineServerInvokerException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// 初始化 <see cref="PipelineServerInvokerException"/> 对象。
        /// </summary>
        /// <param name="cmd">进程命令对象。</param>
        /// <param name="exception">内部错误</param>
        public PipelineServerInvokerException(CmdParam cmd,Exception exception)
            : this($"消息ID：{cmd.CmdId}请求{cmd.TargetTypeName}/{cmd.ServiceName}服务时，发送错误！",exception)
        {
        }

        /// <summary>
        /// 初始化 <see cref="PipelineServerInvokerException"/> 对象。
        /// </summary>
        /// <param name="info"><see cref="T:System.Runtime.Serialization.SerializationInfo" />，它存有有关所引发异常的序列化的对象数据。</param>
        /// <param name="context"><see cref="T:System.Runtime.Serialization.StreamingContext" />，它包含有关源或目标的上下文信息。</param>
        protected PipelineServerInvokerException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
