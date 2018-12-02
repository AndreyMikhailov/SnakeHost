using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace SnakeHost
{
    class Program
    {
        static void Main(string[] args)
        {
            new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseUrls("http://0.0.0.0:80/")
                .ConfigureLogging((hostingContext, logging) =>
                {
                    //logging.SetMinimumLevel(LogLevel.Debug);
                    //logging.AddConsole();
                    //logging.AddDebug();
                })
                .Build()
                .Run();
        }
    }
}