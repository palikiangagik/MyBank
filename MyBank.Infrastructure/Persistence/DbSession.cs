using CorePrimitives;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MyBank.Application.Interfaces;
using System.Data;
using System.Data.Common;

namespace MyBank.Infrastructure.Persistence
{
    public class DbSession : IDbSession
    {
        private readonly MyBankDbContext _dbContext;

        public DbSession(MyBankDbContext dbContext) => _dbContext = dbContext;

        public async Task BeginTransactionAsync()
        {
            await _dbContext.Database.BeginTransactionAsync();
        }

        public async Task RollbackAsync()
        {
            await _dbContext.Database.RollbackTransactionAsync();
        }

        public void Rollback()
        {
            _dbContext.Database.RollbackTransaction();
        }

        public async Task CommitAsync()
        {
            await _dbContext.Database.CommitTransactionAsync();
        }
    }
}
