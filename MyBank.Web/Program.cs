using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyBank.Web.Contracts.Account;
using MyBank.Web.Data;
using System.Threading.Tasks;

namespace MyBank.Web
{
    public class Program
    {
        static public async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // Applying migrations and seeding test data if run in dev environment
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<MyBankWebContext>();
                var accountService = services.GetRequiredService<IAccountService>();
                var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                var logger = services.GetRequiredService<ILogger<TestDataSeeder>>();
                var env = services.GetRequiredService<IHostEnvironment>();

                context.Database.Migrate();
                if (env.IsDevelopment())
                    await new TestDataSeeder(accountService, userManager, logger).Run();
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
