using Dapper;
using MyBank.Domain.Entities;
using MyBank.Application.Interfaces;

namespace MyBank.Infrastructure.Persistence.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly UnitOfWork _uow;

        public TransactionRepository(UnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task AddAsync(Transaction transaction)
        {
            var con = await _uow.GetConnection();

            const string sql = @"
                INSERT INTO dbo.Transactions 
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
                    _ => throw new InvalidOperationException("Unknown transaction type encountered in DB.")
                }
            };

            await con.ExecuteAsync(sql, parameters, transaction: _uow.Transaction);
        }
    }
}
