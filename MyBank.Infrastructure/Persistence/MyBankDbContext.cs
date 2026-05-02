using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;
using System.Reflection;

namespace MyBank.Infrastructure.Persistence
{
    public class MyBankDbContext : IdentityDbContext<IdentityUser>
    {
        public DbConnection Connection => Database.GetDbConnection();
        public DbTransaction? Transaction => Database.CurrentTransaction?.GetDbTransaction();

        public MyBankDbContext(DbContextOptions<MyBankDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public async Task MigrateAsync()
        {
            await Database.MigrateAsync();
            string setupSql = await GetSqlScript("InitialSetup.sql");
            await Connection.ExecuteAsync(setupSql);
        }

        private async Task<string> GetSqlScript(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = $"MyBank.Infrastructure.Migrations.Scripts.{fileName}";

            using Stream stream = assembly.GetManifestResourceStream(resourceName)
                ?? throw new FileNotFoundException($"Can't open the resource: {resourceName}");

            using StreamReader reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }
    }
}
