using System.IO;
using log4net.Core;
using log4net.Layout;
using Newtonsoft.Json;

namespace Joker.MultiProc.PipelineServer.ServerLog
{
    /// <summary>
    /// Json格式日志布局
    /// </summary>
    public class JsonLayout : log4net.Layout.LayoutSkeleton
    {
        /// <summary>
        /// 
        /// </summary>
        public override void ActivateOptions()
        {
        }

        /// <summary>
        /// 格式化
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="loggingEvent"></param>
        public override void Format(TextWriter writer, LoggingEvent loggingEvent)
        {
            var jsonSettings = new JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-dd HH:mm:ss.ffffff",
                DateFormatHandling = DateFormatHandling.IsoDateFormat
            };

            writer.WriteLine("*********************************************************");
            writer.WriteLine(JsonConvert.SerializeObject(loggingEvent.MessageObject, Formatting.Indented, jsonSettings));
        }
    }
}
