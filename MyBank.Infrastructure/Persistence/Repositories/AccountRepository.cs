using CorePrimitives;
using Dapper;
using Microsoft.EntityFrameworkCore;
using MyBank.Application.Interfaces;
using MyBank.Domain.Common;
using MyBank.Domain.Entities;
using MyBank.Domain.ValueObjects;

namespace MyBank.Infrastructure.Persistence.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly MyBankDbContext _db;

        public AccountRepository(MyBankDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Account account)
        {
            const string sql = @"
                INSERT INTO Accounts (Id, Code, ClientId, Balance, IsClosed) 
                VALUES (@Id, @Code, @ClientId, @Balance, @IsClosed)";

            await _db.Connection.ExecuteAsync(sql, new
            {
                Id = account.Id.Value,
                Code = account.Code,
                ClientId = account.ClientId.Value,
                Balance = account.Balance.Value,
                IsClosed = account.IsClosed
            }, transaction: _db.Transaction);
        }

        public async Task<Result<Account>> GetAsync(IntId clientId, IntId accountId, bool blockUntilUpdate)
        {
            const string sqlNonBlocking = $"SELECT * FROM Accounts WHERE Id = @Id AND ClientId = @ClientId";
            const string sqlBlocking = $"SELECT * FROM Accounts WITH (UPDLOCK, ROWLOCK) WHERE Id = @Id AND ClientId = @ClientId";
            string sql = blockUntilUpdate ? sqlBlocking : sqlNonBlocking;

            var row = await _db.Connection.QuerySingleOrDefaultAsync<dynamic>(sql,
                new { Id = accountId.Value, ClientId = clientId.Value }, 
                transaction: _db.Transaction);

            if (row == null)
                return Errors.AccountNotFound;

            return new Account(row.Id, row.Code, row.ClientId, row.Balance, row.IsClosed);
        }

        public async Task<Result<Account>> GetAsync(IntId accountId, bool blockUntilUpdate)
        {
            const string sqlNonBlocking = "SELECT * FROM Accounts WHERE Id = @Id";
            const string sqlBlocking = "SELECT * FROM Accounts WITH (UPDLOCK, ROWLOCK) WHERE Id = @Id";
            string sql = blockUntilUpdate ? sqlBlocking : sqlNonBlocking;

            var row = await _db.Connection.QuerySingleOrDefaultAsync<dynamic>(sql,
                new { Id = accountId.Value },
                transaction: _db.Transaction);

            if (row == null)
                return Errors.AccountNotFound;

            return new Account(row.Id, row.Code, row.ClientId, row.Balance, row.IsClosed);
        }

        public async Task UpdateAsync(Account account)
        {
            const string sql = @"
                UPDATE dbo.Accounts 
                SET Code = @Code,
                    ClientId = @ClientId,
                    Balance = @Balance, 
                    IsClosed = @IsClosed 
                WHERE Id = @Id";

            await _db.Connection.ExecuteAsync(sql, new
            {
                Id = account.Id.Value, 
                Code = account.Code,
                ClientId = account.ClientId.Value, 
                Balance = account.Balance.Value,
                IsClosed = account.IsClosed
            }, transaction: _db.Transaction);
        }
    }
}
