using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;

namespace Joker.MultiProc.PipelineServer.ProcessService
{
    /// <summary>
    /// 服务方法描述类
    /// </summary>
    internal class LocalMethodDescription : IMethodDescription
    {
        private readonly MethodInfo _sourceMethodInfo;
        private readonly ParameterInfo[] _sourceMethodParameters;
        private readonly MethodInfo _targetMethodInfo;
        private readonly ParameterInfo[] _targetParameterInfos;
        private readonly Type _targetType;
        private readonly MultiProcessAttribute _processAttribute;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="processAttribute"></param>
        public LocalMethodDescription(MethodInfo methodInfo, MultiProcessAttribute processAttribute)
        {
            this._sourceMethodInfo = methodInfo;
            this._processAttribute = processAttribute;
            this._targetType = _processAttribute.ServiceType;

            if (this._targetType.IsGenericType)
            {
                throw new InvalidOperationException($@"初始化失败，不支持泛型类型。{Environment.NewLine}{_targetType.FullName}");
            }

            MethodInfo method = this._targetType.GetMethod(this._sourceMethodInfo.Name, BindingFlags.Instance | BindingFlags.Public);
            if (method == null)
            {
                throw new InvalidOperationException($@"初始化失败，类型不存在指定的方法。{Environment.NewLine}类型名：{_targetType.FullName}{Environment.NewLine}方法名：{this._sourceMethodInfo.Name}");
            }

            if (method.IsGenericMethod)
            {
                throw new InvalidOperationException($@"初始化失败，不支持泛型方法。{Environment.NewLine}类型名：{_targetType.FullName}{Environment.NewLine}方法名：{this._sourceMethodInfo.Name}");
            }
            this._targetMethodInfo = method;
            this._targetParameterInfos = method.GetParameters();
            this._sourceMethodParameters = this._sourceMethodInfo.GetParameters();
            if (this._targetParameterInfos.Length != this._sourceMethodParameters.Length)
            {
                throw new InvalidOperationException($@"初始化失败，方法参数个数与接口定义不符。{Environment.NewLine}类型名：{_targetType.FullName}{Environment.NewLine}方法名：{this._sourceMethodInfo.Name}");
            }
        }

        /// <summary>
        /// 创建服务对象
        /// </summary>
        /// <returns></returns>
        private object CreateServiceObject()
        {
            //暂时不用IOC接入。直接创建无参对象
            return Activator.CreateInstance(_targetType);
        }
        
        /// <summary>
        /// 服务方法回调
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public object Call(object[] args)
        {
            object obj = CreateServiceObject();
            List<object> objectList = new List<object>();
            for (int index = 0 ; index < this._targetParameterInfos.Length ; ++index)
            {
                var targetParameterInfo = this._targetParameterInfos[index];
                var sourceMethodParameter = this._sourceMethodParameters[index];
                var sourceObject = args[index];
                if (sourceObject == null && targetParameterInfo.ParameterType.IsValueType)
                {
                    throw new InvalidCastException(
                        $@"调用错误，调用方传入null，接收方为值类型。{Environment.NewLine}{string.Join(Environment.NewLine, new[]
                        {
                            $@"类型名：{this._sourceMethodInfo.DeclaringType?.FullName}",
                            $@"方法名：{_sourceMethodInfo.Name}",
                            $@"参数名：{sourceMethodParameter.Name}"
                        })}");
                }

                if (sourceObject == null ||
                    !this.NeedConvert(sourceObject.GetType(), targetParameterInfo.ParameterType))
                {
                    // 不需要执行类型转换的场景下，直接将原始参数给作为目标方法的参数
                    objectList.Add(sourceObject);
                } else
                {
                    // RemoteService接口定义类型（source）-》PubService接口类型else
                    // 需要类型转换时，执行快速类型转换
                    objectList.Add(this.FastConvertType(targetParameterInfo.ParameterType, sourceObject));
                }
            }

            // 执行pubService方法调用
            // 如果方法发生异常，直接抛出
            var returnObject = this._targetMethodInfo.Invoke(obj, objectList.ToArray());
            if (returnObject == null)
            {
                return null;
            }

            // 返回类型转换
            // PubService返回类型=》RemoteService返回类型
            if (this.NeedConvert(returnObject.GetType(), this._sourceMethodInfo.ReturnType))
            {
                returnObject = this.FastConvertType(this._sourceMethodInfo.ReturnType, returnObject);
            }

            return returnObject;
        }

        /// <inheritdoc />
        public string ServiceName => ProcessEnvironment.FormatServerName(this._processAttribute?.ServiceName);

        /// <inheritdoc />
        public int ServiceCount => this._processAttribute.ServiceCount;

        /// <inheritdoc />
        public string TypeName => _sourceMethodInfo?.DeclaringType?.FullName;

        /// <inheritdoc />
        public string MethodName => _sourceMethodInfo?.Name;

        /// <summary>
        /// 是否需要类型转换
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        private bool NeedConvert(Type sourceType, Type targetType)
        {
            // 以下两种场景不需要做类型转换：
            // 1、类型一致（例如都是string）
            // 2、源类型是目标类型的子类，
            return sourceType != targetType && !targetType.IsAssignableFrom(sourceType);
        }

        /// <summary>
        /// 快速类型转换
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="sourceObject"></param>
        /// <returns></returns>
        private object FastConvertType(Type targetType, object sourceObject)
        {
            if (sourceObject == null)
                return null;
            return ConvertToTargetType(sourceObject, targetType);
        }

        /// <summary>
        /// 对象转换为目标类型
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        internal static object ConvertToTargetType(object obj, Type targetType)
        {
            var jsonData = JsonConvert.SerializeObject(obj);
            return JsonConvert.DeserializeObject(jsonData, targetType);
        }
    }

}
