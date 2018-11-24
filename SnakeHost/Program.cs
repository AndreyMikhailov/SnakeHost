using System.IO;
using Microsoft.AspNetCore.Hosting;

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
                .UseUrls("http://localhost:5000/")
                .ConfigureLogging((hostingContext, logging) =>
                {
                    /*logging.SetMinimumLevel(LogLevel.Debug);
                    logging.AddConsole();
                    logging.AddDebug();*/
                })
                .Build()
                .Run();
        }
    }
}