using CorePrimitives;
using MyBank.Domain.ValueObjects;
using MyBank.Domain.Entities;

namespace MyBank.Application.Interfaces
{
    public interface IAccountRepository
    {
        public Task<IntId> GetNextIdAsync();
        public Task<StringId> GetNextCodeAsync();
        public Task AddAsync(Account account);
        public Task<Result<Account>> GetAsync(StringId userId, IntId accountId);
        public Task<Result<Account>> GetAsync(IntId accountId);
        public Task UpdateAsync(Account account);
    }
}
