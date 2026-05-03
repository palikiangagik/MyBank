using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using MyBank.Infrastructure.Persistence;
using System.Threading.Tasks;

namespace MyBank.Web
{
    public class Program
    {
        static public async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await host.InitializeMyBankDatabaseAsync();
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
