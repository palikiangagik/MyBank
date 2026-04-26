using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using MyBank.Application.Interfaces;

namespace MyBank.Infrastructure.Persistence
{
    public class MigrationService
    {
        private readonly MyBankIdentityDbContext _identityContext;
        private readonly IUnitOfWork _unitOfWork;

        public MigrationService(MyBankIdentityDbContext identityContext, IUnitOfWork unitOfWork)
        {
            _identityContext = identityContext;
            _unitOfWork = unitOfWork;
        }

        public async Task ApplyMigrationsAsync()
        {
            _identityContext.Database.Migrate();

            string setupSql = await GetSqlScript("InitialSetup.sql");
            await _unitOfWork.Connection.ExecuteAsync(setupSql);
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
