using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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

            // Applying migrations and seeding test data if run in dev environment
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var env = services.GetRequiredService<IHostEnvironment>();
                var db = services.GetRequiredService<MyBankDbContext>();
                var seeder = services.GetRequiredService<DevelopmentDbSeeder>();

                await db.MigrateAsync();
                if (env.IsDevelopment())
                    await seeder.Run();
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
