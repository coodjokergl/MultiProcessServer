using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Joker.MultiProc.PipelineServer.Pipeline;
using Newtonsoft.Json;

namespace Joker.MultiProc.PipelineServer.ProcessService
{
    /// <summary>
    /// 启动
    /// </summary>
    internal static class StartUp
    {
        /// <summary>
        /// 加载服务
        /// </summary>
        /// <param name="cmdLine">命令参数</param>
        public static async void RunServer(string cmdLine)
        {
            var info = StartUpInfo.DeEncode(cmdLine);
            ProcessEnvironment.StartInfo = info;
            //子进程调试开关.放开注释可调试
            //EnableDebug(info.IsDebug);

            //服务模式
            ProcessEnvironment.IsServer = true;

            foreach (var file in info.Plugins)
            {
                Assembly.LoadFile(file);
            }

            //加载服务清单
            try
            {
                RegisterProcessService.Register();

                //开启后台异步服务
                await PipelineServerPool.CreateServerPipeLineAsync(info.ServerName);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        /// <summary>
        /// 调试开关
        /// </summary>
        /// <param name="isDebug"></param>
        private static async void EnableDebug(bool isDebug)
        {
            if(!isDebug) return;

            //如果是调试状态，等待调试接入
            await Task.Run(() => {
                while (!ProcessEnvironment.IsDebug)
                {
                    Thread.Sleep(1 * 1000);
                }
                //触发断点
                Debugger.Break();
            });
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void StartUpServer(string name, int processCount)
        {
            lock (ProcessEnvironment.AsyncLockObj)
            {
                if (!ValidateStartUpServer(name,processCount)) return;

                var startInfo = new StartUpInfo();
                startInfo.Plugins = AppDomain.CurrentDomain.GetAssemblies().Where(q => q.IsPipeServerAssembly()).Select(q => q.Location).ToList();
                startInfo.ServerName = name;
                startInfo.IsDebug = ProcessEnvironment.IsDebug;//调试状态传递

                var exeLocation = Assembly.GetExecutingAssembly().Location;
                var workingDirectory = Path.GetDirectoryName(exeLocation);

                //获取已存在的数量
                ProcessEnvironment.Servers.TryGetValue(name, out List<Process> existProcesses);
                existProcesses = existProcesses ?? new List<Process>();

                if (ProcessEnvironment.IsDebug)
                {
                    processCount = 1;
                }

                //排除已经退出的进程（被用户或者其他程序杀死的进程）。排除后可以重新开启服务进程
                existProcesses = existProcesses.Where(q => !q.HasExited).ToList();

                //启动多进程
                for (int i = existProcesses.Count; i < processCount ; i++)
                {
                    var processInfo = new ProcessStartInfo(exeLocation);
                    processInfo.WorkingDirectory = workingDirectory ?? string.Empty;
                    processInfo.UseShellExecute = false;
                    processInfo.Arguments = startInfo.Encode();
                    processInfo.CreateNoWindow = true;
                    
                    var process = Process.Start(processInfo);
                    if (process == null)
                    {
                        throw new Exception($@"子进程启动失败!{startInfo}");
                    }
                    existProcesses.Add(process);
                    //服务数量
                    ProcessEnvironment.Servers[name] = existProcesses;

                    process.Exited += new EventHandler((sender, arg) => {
                        lock (ProcessEnvironment.AsyncLockObj)
                        {
                            if (ProcessEnvironment.Servers.TryGetValue(name, out List<Process> processes))
                            {
                                processes.Remove(process);
                            }
                        }
                    });

                    //等待服务启动
                    Thread.Sleep(100);
                }
            }
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        public static bool ValidateStartUpServer(string name,int count)
        {
            //当前已是服务进程，不能启动子进程
            if (ProcessEnvironment.IsServer) return false;

            //服务已存在，无需重启
            if (ProcessEnvironment.Servers.TryGetValue(name, out List<Process> processes) && processes.Count(q => !q.HasExited) >= count)
            {
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// 启动信息
    /// </summary>
    [Serializable]
    public class StartUpInfo
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// 插件
        /// </summary>
        public List<string> Plugins { get; set; }

        /// <summary>
        /// 是否调试状态
        /// </summary>
        public bool IsDebug { get; set; }

        /// <summary>
        /// 服务前缀
        /// </summary>
        public Guid ServerPrefix { get; }

        /// <summary>
        /// 编码
        /// </summary>
        /// <returns></returns>
        public string Encode()
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this)));
        }

        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static StartUpInfo DeEncode(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            } else
            {
                return JsonConvert.DeserializeObject<StartUpInfo>(
                    Encoding.UTF8.GetString(Convert.FromBase64String(value)));
            }
        }

        /// <summary>
        /// 解码
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
