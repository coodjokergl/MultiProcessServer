using System;
using Joker.MultiProc.PipelineServer;
using Joker.MultiProc.PipelineServer.ProcessService;

namespace Joker.MultiProc.Demo
{
    /// <summary>
    /// 
    /// </summary>
    internal class Calculator:ICalculator,IProcessService
    {
        /// <inheritdoc cref="Sum"/>
        public virtual int Sum(int a, int b)
        {
            Console.WriteLine(ProcessEnvironment.IsDebug);
            return a + b;
        }

        /// <inheritdoc />
        public Info Get()
        {
            return new Info()
            {
                Name = "gl"
            };
        }

        /// <inheritdoc />
        public void Task()
        {
            throw new Exception("服务错误！");
        }
    }
}
