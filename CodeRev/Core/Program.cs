using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
            CompilerService.Program.Main(args);
            TrackerService.Program.Main(args);
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
