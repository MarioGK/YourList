using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace YourList.Worker
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSystemd()
                .ConfigureServices((hostContext, services) => { services.AddHostedService<Worker>(); });
        }
    }
}