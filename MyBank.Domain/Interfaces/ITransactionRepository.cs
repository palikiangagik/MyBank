using MyBank.Domain.Entities;
using MyBank.Domain.ValueObjects;

namespace MyBank.Domain.Interfaces
{
    public interface ITransactionRepository
    {
        public Task<IntId> GetNextIdAsync();
        public Task AddAsync(Transaction transaction);
    }
}
