using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Joker.MultiProc.PipelineServer.ProcessService
{
    /// <summary>
    /// 远程调用注册服务
    /// </summary>
    public static class RegisterProcessService
    {
        private static bool _isInit = false;

        /// <summary>
        /// 注册服务
        /// </summary>
        public static void Register()
        {
            if(_isInit) return;

            foreach (var assembly in GetAssemblies())
            {
                foreach (var type in assembly.GetExportedTypes().Where(q=>q.IsPipeServer()))
                {
                    ProcessServiceContainer.Instance.Register(type);
                }
            }

            _isInit = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal static List<Assembly> GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies().Where(q => q.IsPipeServerAssembly()).ToList();

        }
    }

}
