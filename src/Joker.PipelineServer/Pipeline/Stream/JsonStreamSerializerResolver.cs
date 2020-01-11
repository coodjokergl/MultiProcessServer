namespace Joker.MultiProc.PipelineServer.Pipeline.Stream
{
    /// <summary>
    /// 
    /// </summary>
    public static class JsonStreamSerializerResolver
    {
        /// <summary>
        /// 序列化
        /// </summary>
        public static IStreamSerializer Serializer { get; private set; } = new JsonStreamSerializer();

        /// <summary>
        /// 注入自定义的序列化工具
        /// </summary>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public static IStreamSerializer Register(IStreamSerializer  serializer)
        {
            var originSerializer = Serializer;
            Serializer = serializer;
            return originSerializer;
        }
    }
}
