using System.Linq;
using Joker.MultiProc.PipelineServer.ProcessService;

namespace Joker.MultiProc.PipelineServer
{
    class Program
    {
        static int Main(string[] args)
        {
            //启动服务
            var existCode = StartUp.RunServer(args.FirstOrDefault());
            return existCode.Result;
        }
    }
}
