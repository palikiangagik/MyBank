using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace MyBank.Portal
{
    public class Program
    {
        static public async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (var seeder = new TestDataSeeder(host)) 
                await seeder.Run();
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
