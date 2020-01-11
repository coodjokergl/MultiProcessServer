using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Joker.MultiProc.PipelineServer
{
    /// <summary>
    /// 进程环境变量
    /// </summary>
    public static class ProcessEnvironment
    {
        /// <summary>
        /// 是否服务端
        /// </summary>
        public static bool IsServer { get; internal set; } = false;

        /// <summary>
        /// 是否调试
        /// </summary>
        public static bool IsDebug => Debugger.IsAttached;

        /// <summary>
        /// 已启动的服务
        /// </summary>
        public static Dictionary<string, int> Servers { get; } = new Dictionary<string, int>();

        /// <summary>
        /// 统一配置服务进程数量
        /// </summary>
        public static ushort? ServiceCount { get; set; }

        /// <summary>
        /// 线程互斥对象
        /// </summary>
        internal static object AsyncLockObj = new object();


        /// <summary>
        /// 是否激活
        /// </summary>
        internal static void TriggerActive(bool isIn)
        {
            lock (AsyncLockObj)
            {
                //不用控制线程安全
                LastActiveTime = DateTime.Now;
                ActiveCount += isIn ? 1 : -1;
            }
        }

        /// <summary>
        /// 尝试退出
        /// </summary>
        internal static void TryExist()
        {
            var lastDataTime = LastActiveTime;

            if (DateTime.Now.AddMinutes(-3) > lastDataTime && ActiveCount <= 0)
            {
                // 超时未服务的，自动退出
                System.Environment.Exit(0);
            }
        }
        /// <summary>
        /// 激活数量
        /// </summary>
        private static int ActiveCount {get; set; }

        /// <summary>
        /// 激活时间
        /// </summary>
        private static DateTime LastActiveTime { get; set; }
    }
}
