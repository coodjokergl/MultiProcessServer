using System;
using Joker.MultiProc.PipelineServer.ProcessService;

namespace Joker.MultiProc.Demo
{
    /// <summary>
    /// 
    /// </summary>
    [MultiProcess(typeof(Calculator), "测试服务", 3)]
    public interface ICalculator : IProcessService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        int Sum(int a, int b);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Info Get();

        /// <summary>
        /// 
        /// </summary>
        void Task();
    }

    /// <summary>
    /// 信息
    /// </summary>
    [Serializable]
    public class Info
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $@"{Name} _ 测试";
        }
    }
}
