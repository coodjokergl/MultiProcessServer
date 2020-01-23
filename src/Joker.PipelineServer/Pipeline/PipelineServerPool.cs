using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;
using Joker.MultiProc.PipelineServer.ServerLog;

namespace Joker.MultiProc.PipelineServer.Pipeline
{
    /// <summary>
    /// 管道服务的进程池
    /// </summary>
    internal class PipelineServerPool
    {
        /// <summary>
        /// 用于存储和管理管道的进程池
        /// </summary>
        private static readonly ConcurrentDictionary<Guid, PipelineBase> ServerPool = new ConcurrentDictionary<Guid, PipelineBase>();

        /// <summary>
        /// 最大的服务数量
        /// </summary>
        private const int MaxNumberServer = 100;

        /// <summary>
        /// 创建一个新的管道
        /// </summary>
        /// <param name="serverName">服务名称</param>
        /// <returns></returns>
        public static Guid CreatePipeLine(string serverName)
        {
            lock (ServerPool)
            {
                if (ServerPool.Count < MaxNumberServer)
                {
                    PipelineBase pipe = new PipelineServer();

                    //添加到程序池中
                    ServerPool.TryAdd(pipe.Id, pipe);

                    //启动服务
                    pipe.Start(serverName);

                    return pipe.Id;
                } else
                {
                    throw new Exception($@"管线服务超过最大数量!管道池添加新管道 当前管道总数{ServerPool.Count}。");
                }
            }
        }

        /// <summary>
        /// 根据ID从管道池中释放一个管道
        /// </summary>
        private static void DisposablePipeLine(Guid id)
        {
            lock (ServerPool)
            {
                Debugger.Log(1,"管道池",$"开始尝试释放,管道{id}");
                Logger.Log.Info($@"开始尝试释放,管道{id}");
                if (ServerPool.TryRemove(id, out PipelineBase pipe))
                {
                    Debugger.Log(1,"管道池",$"管道{id},已经关闭,并完成资源释放！");
                    Logger.Log.Info($@"管道{id},已经关闭,并完成资源释放！");
                    if (ServerPool.IsEmpty)
                    {
                        pipe.NextPipeServerAsync(); //开启一个新的空闲服务管道，保持服务可用
                    }
                }
                else
                {
                    Debugger.Log(1,"服务池",$"未找到ID为{id}的管道服务，释放无效！");
                    Logger.Log.Info($@"未找到ID为{id}的管道服务，释放无效！");
                }
            }
        }

        /// <summary>
        /// (异步)创建一个新的管道进程
        /// </summary>
        public static async Task<Guid> CreateServerPipeLineAsync(string serverName)
        {
            return await Task.Run(()=>
            {
                lock (ServerPool)
                {
                    if (ServerPool.Count < MaxNumberServer)
                    {
                        PipelineBase pipe = new PipelineServer();

                        //添加到程序池中
                        ServerPool.TryAdd(pipe.Id, pipe);

                        //启动服务
                        pipe.StartAsync(serverName);

                        return pipe.Id;
                    } else
                    {
                        throw new Exception($@"管线服务超过最大数量!管道池添加新管道 当前管道总数{ServerPool.Count}。");
                    }
                }
            });
        }

        /// <summary>
        /// (异步)根据ID从管道池中释放一个管道
        /// </summary>
        /// <param name="id"></param>
        public static async void DisposablePipeLineAsync(Guid id) => await Task.Run(() => { DisposablePipeLine(id); });


        public static PipelineBase Get(Guid id)
        {
            lock (ServerPool)
            {
                if (ServerPool.TryGetValue(id, out PipelineBase pipe))
                {
                    return pipe;
                }
            }

            return null;
        }
    }
}
