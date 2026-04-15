using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyBank.Portal.Data;
using System.Threading.Tasks;

namespace MyBank.Portal
{
    public class Program
    {
        static public async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // Applying migrations and seeding test data
            using (var scope = host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<MyBankPortalContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                context.Database.Migrate();
                await new TestDataSeeder(context, userManager, logger).Run();
            }

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
