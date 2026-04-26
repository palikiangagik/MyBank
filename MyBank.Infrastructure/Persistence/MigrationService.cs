using Dapper;
using Microsoft.EntityFrameworkCore;
using MyBank.Application.Interfaces;
using System.Reflection;

namespace MyBank.Infrastructure.Persistence
{
    public class MigrationService
    {
        private readonly MyBankIdentityDbContext _identityContext;
        private readonly UnitOfWork _unitOfWork;

        public MigrationService(MyBankIdentityDbContext identityContext, UnitOfWork unitOfWork)
        {
            _identityContext = identityContext;
            _unitOfWork = unitOfWork;
        }

        public async Task ApplyMigrationsAsync()
        {
            await _identityContext.Database.MigrateAsync();

            string setupSql = await GetSqlScript("InitialSetup.sql");
            var conn = await _unitOfWork.GetConnection();
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
