using MyBank.Domain.Interfaces;
using MyBank.Domain.ValueObjects;
using MyBank.Domain.Entities;

namespace MyBank.Infrastructure.Persistence.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        public async Task<IntId> GetNextIdAsync()
        {
            return 0;
        }

        public async Task AddAsync(Transaction transaction)
        {
            return;
        }
    }
}
