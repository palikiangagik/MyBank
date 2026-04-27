using Dapper;
using Microsoft.EntityFrameworkCore;
using MyBank.Application.Interfaces;
using System.Reflection;

namespace MyBank.Infrastructure.Persistence
{
    public class MigrationService
    {
        private readonly MyBankIdentityDbContext _identityContext;
        private readonly DbSession _db;

        public MigrationService(MyBankIdentityDbContext identityContext, DbSession db)
        {
            _identityContext = identityContext;
            _db = db;
        }

        public async Task ApplyMigrationsAsync()
        {
            await _identityContext.Database.MigrateAsync();

            string setupSql = await GetSqlScript("InitialSetup.sql");
            var conn = await _db.GetConnection();
            await conn.ExecuteAsync(setupSql);
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
