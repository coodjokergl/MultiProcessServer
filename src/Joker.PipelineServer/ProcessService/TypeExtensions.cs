using System;
using System.Reflection;

namespace Joker.MultiProc.PipelineServer.ProcessService
{
    internal static class TypeExtensions
    {
        /// <summary>
        /// 判断该方法是否为管道方法。
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        public static bool IsPipelineMethod(this MethodInfo methodInfo)
        {
            //如果不是公有方法，直接返回false
            if (methodInfo.IsPublic == false) 
                return false;
            if (methodInfo.IsSpecialName)
                return false;

            // 如果是Object类上面的方法，直接跳过。
            if (methodInfo.GetBaseDefinition().DeclaringType == typeof(object))
                return false;

            // 如果是final方法或者不是需方法，直接返回false
            if (methodInfo.IsFinal || methodInfo.IsVirtual == false)
                return false;

            return true; //return me
        }

        /// <summary>
        /// 判断该方法是否为能扩展的方法。
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        public static bool IsAllowExtendMethod(this MethodInfo methodInfo)
        {
            //如果不是公有方法，直接返回false
            if (methodInfo.IsPublic == false) //公有方法。
                return false;
            if (methodInfo.IsSpecialName)
                return false;

            // 如果是Object类上面的方法，直接跳过。
            if (methodInfo.GetBaseDefinition().DeclaringType == typeof(object))
                return false;
            // 如果是final方法或者不是需方法，直接返回false
            if (methodInfo.IsFinal || methodInfo.IsVirtual == false)
                return false;

            return true; 
        }

        /// <summary>
        /// 是否管道服务
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static bool IsPipeServerAssembly(this Assembly assembly)
        {
            var attr = assembly.GetCustomAttribute<ProcessPluginServerAttribute>();
            return attr != null;
        }

        /// <summary>
        /// 多进程服务
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsPipeServer(this Type type)
        {
            var attr = type.GetCustomAttribute<MultiProcessAttribute>();
            if (attr == null) return false;

            if (!typeof(IProcessService).IsAssignableFrom(type))
            {
                return false;
            }

            return true;
        }
    }
}
