using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Data.Common;
using System.Reflection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MyBank.Infrastructure.Persistence
{
    public static class DatabaseInitializator
    {
        public static async Task InitializeMyBankDatabaseAsync(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            var env = services.GetRequiredService<IHostEnvironment>();
            var db = services.GetRequiredService<MyBankDbContext>();
            var seeder = services.GetRequiredService<DevelopmentDbSeeder>();

            await db.Database.MigrateAsync();
            await db.Connection.ExecuteSqlScript("InitialSetup.sql");

            if (env.IsDevelopment())
                await seeder.Run();
        }

        private static async Task ExecuteSqlScript(this DbConnection connection, string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = $"MyBank.Infrastructure.Migrations.Scripts.{fileName}";

            using Stream stream = assembly.GetManifestResourceStream(resourceName)
                ?? throw new FileNotFoundException($"Can't open the resource: {resourceName}");

            using StreamReader reader = new StreamReader(stream);
            string sql = await reader.ReadToEndAsync();
            await connection.ExecuteAsync(sql);
        }
    }
}
