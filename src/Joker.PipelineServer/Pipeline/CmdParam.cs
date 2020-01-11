using System;

namespace Joker.MultiProc.PipelineServer.Pipeline
{
    /// <summary>
    /// 命令参数
    /// </summary>
    [Serializable]
    public class CmdParam
    {
        /// <summary>
        /// 命令主键
        /// </summary>
        public Guid CmdId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// 目标类型
        /// </summary>
        public string TargetTypeName { get; set; }

        /// <summary>
        /// 目标服务名称
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public object[] Args { get; set; }

        /// <summary>
        /// 异常
        /// </summary>
        public Exception ErrorException { get; set; }

        /// <summary>
        /// 对象
        /// </summary>
        public object Result { get; set; }
    }
}
