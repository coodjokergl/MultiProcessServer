using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Joker.MultiProc.PipelineServer.ProcessService;
using Joker.MultiProc.PipelineServer.ServerLog;

namespace Joker.MultiProc.PipelineServer
{
    /// <summary>
    /// 进程环境变量
    /// </summary>
    public static class ProcessEnvironment
    {
        static readonly Guid _serverPrefixInstance = Guid.NewGuid();

        /// <summary>
        /// 服务前缀
        /// </summary>
        internal static Guid ServerPrefix => StartInfo?.ServerPrefix ?? _serverPrefixInstance;

        /// <summary>
        /// 启动参数
        /// </summary>
        internal static StartUpInfo StartInfo { get; set; }

        /// <summary>
        /// 是否服务端
        /// </summary>
        public static bool IsServer { get; internal set; } = false;

        /// <summary>
        /// 最大的延迟时效
        /// 当服务没有被链接激活时，最大的进程保留时间。单位为分钟。当超过该设定时间，服务没有被调用激活，则自动关闭进程以释放资源
        /// </summary>
        public static int MaxDelayTime { get; set; } = 3;

        /// <summary>
        /// 是否调试
        /// </summary>
        public static bool IsDebug => Debugger.IsAttached;

        /// <summary>
        /// 已启动的服务
        /// </summary>
        public static Dictionary<string, List<Process>> Servers { get; } = new Dictionary<string, List<Process>>();

        /// <summary>
        /// 统一配置服务进程数量
        /// </summary>
        public static ushort? ServiceCount { get; set; }

        /// <summary>
        /// 线程互斥对象
        /// </summary>
        internal static object AsyncLockObj = new object();

        /// <summary>
        /// 格式化服务名称
        /// </summary>
        /// <param name="serverName"></param>
        /// <returns></returns>
        internal static string FormatServerName(string serverName)
        {
            //为调试方便这里不使用匿名服务名称
            //return Convert.ToBase64String(Encoding.UTF8.GetBytes($@"{serverName}_{ServerPrefix}"));
            return $@"{serverName}_{ServerPrefix}";
        }

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
        internal static bool TryExist()
        {
            var lastDataTime = LastActiveTime;

            if (DateTime.Now.AddMinutes(MaxDelayTime * -1) > lastDataTime && ActiveCount <= 0 /* && !StartInfo.IsDebug*/)
            {
                Debugger.Log(1,"退出","即将关闭服务程序！");
                Logger.Log.Info($@"{MaxDelayTime}分钟未激活进程服务，即将关闭服务程序！");

                // 超时未服务的，自动退出
                System.Environment.Exit(0);

                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 激活数量
        /// </summary>
        private static int ActiveCount {get; set; }

        /// <summary>
        /// 激活时间
        /// </summary>
        private static DateTime LastActiveTime { get; set; } = DateTime.Now;
    }
}
