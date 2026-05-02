using Dapper;
using Microsoft.EntityFrameworkCore;
using MyBank.Application.Interfaces;
using MyBank.Domain.Entities;
using System.Data.Common;

namespace MyBank.Infrastructure.Persistence.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly MyBankDbContext _db;

        public TransactionRepository(MyBankDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Transaction transaction)
        {
            const string sql = @"
                INSERT INTO Transactions 
                (Id, CreatedAt, [Type], Amount, SenderAccountId, RecipientAccountId, AccountId) 
                VALUES 
                (@Id, @CreatedAt, @Type, @Amount, @SenderAccountId, @RecipientAccountId, @AccountId)";

            var parameters = new
            {
                Id = transaction.Id.Value,
                CreatedAt = transaction.CreatedAt,
                Type = (int)transaction.Type,
                Amount = transaction.Amount.Value, 
                SenderAccountId = transaction is TransferTransaction t ? t.SenderAccountId.Value : (int?)null,
                RecipientAccountId = transaction is TransferTransaction r ? r.RecipientAccountId.Value : (int?)null,
                AccountId = transaction switch
                {
                    WithdrawalTransaction w => w.AccountId.Value,
                    DepositTransaction d => d.AccountId.Value,
                    TransferTransaction tr => (int?)null,
                    _ => throw new InvalidOperationException("Attempted to add an unknown transaction type to the database.")
                }
            };

            await _db.Connection.ExecuteAsync(sql, parameters, transaction: _db.Transaction);
        }
    }
}
