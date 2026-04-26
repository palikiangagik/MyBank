using Dapper;
using CorePrimitives;
using Microsoft.EntityFrameworkCore.Metadata;
using MyBank.Domain.Entities;
using MyBank.Domain.Interfaces;
using MyBank.Domain.ValueObjects;
using MyBank.Domain.Common;

namespace MyBank.Infrastructure.Persistence.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UnitOfWork _uow;

        public AccountRepository(UnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<IntId> GetNextIdAsync()
        {
            var con = await _uow.GetConnection();
            const string sql = "SELECT NEXT VALUE FOR dbo.IdSequence";
            var nextId = await con.ExecuteScalarAsync<int>(sql, transaction: _uow.Transaction);
            return nextId;
        }

        public async Task AddAsync(Account account)
        {
            var con = await _uow.GetConnection();

            const string sql = @"
                INSERT INTO dbo.Accounts (Id, Code, UserId, Balance, IsClosed) 
                VALUES (@Id, @Code, @UserId, @Balance, @IsClosed)";

            await con.ExecuteAsync(sql, new
            {
                Id = account.Id.Value,
                Code = account.Code.Value,
                UserId = account.UserId.Value,
                Balance = account.Balance.Value,
                IsClosed = account.IsClosed
            }, transaction: _uow.Transaction);
        }

        public async Task<Result<Account>> GetAsync(StringId userId, IntId accountId, bool blockUntilUpdate)
        {
            var con = await _uow.GetConnection();


            const string sqlNonBlocking = $"SELECT * FROM dbo.Accounts WHERE Id = @Id AND UserId = @UserId";
            const string sqlBlocking = $"SELECT * FROM dbo.Accounts WITH (UPDLOCK, ROWLOCK) WHERE Id = @Id AND UserId = @UserId";
            string sql = blockUntilUpdate ? sqlBlocking : sqlNonBlocking;

            var row = await con.QuerySingleOrDefaultAsync<dynamic>(sql,
                new { Id = accountId.Value, UserId = userId.Value }, 
                transaction: _uow.Transaction);

            if (row == null)
                return Failures.AccountNotFound;

            // manual mapping, because we have no SqlMapper defined
            return new Account(row.Id, row.Code, row.UserId, row.Balance, row.IsClosed);
        }

        public async Task<Result<Account>> GetAsync(IntId accountId, bool blockUntilUpdate)
        {
            var con = await _uow.GetConnection();

            const string sqlNonBlocking = "SELECT * FROM dbo.Accounts WHERE Id = @Id";
            const string sqlBlocking = "SELECT * FROM dbo.Accounts WITH (UPDLOCK, ROWLOCK) WHERE Id = @Id";
            string sql = blockUntilUpdate ? sqlBlocking : sqlNonBlocking;

            var row = await con.QuerySingleOrDefaultAsync<dynamic>(sql,
                new { Id = accountId.Value },
                transaction: _uow.Transaction);

            if (row == null)
                return Failures.AccountNotFound;

            // manual mapping, because we have no SqlMapper defined
            return new Account(row.Id, row.Code, row.UserId, row.Balance, row.IsClosed);
        }

        public async Task UpdateAsync(Account account)
        {
            var con = await _uow.GetConnection();

            const string sql = @"
                UPDATE dbo.Accounts 
                SET Code = @Code,
                    UserId = @UserId,
                    Balance = @Balance, 
                    IsClosed = @IsClosed 
                WHERE Id = @Id";

            await con.ExecuteAsync(sql, new
            {
                Id = account.Id.Value, 
                Code = account.Code.Value,
                UserId = account.UserId.Value, 
                Balance = account.Balance.Value,
                IsClosed = account.IsClosed
            }, transaction: _uow.Transaction);
        }
    }
}
