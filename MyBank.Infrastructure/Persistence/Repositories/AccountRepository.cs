using CorePrimitives;
using MyBank.Domain.Entities;
using MyBank.Domain.Interfaces;
using MyBank.Domain.ValueObjects;

namespace MyBank.Infrastructure.Persistence.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        public async Task<IntId> GetNextIdAsync()
        {
            return 0;
        }

        public async Task<StringId> GetNextCodeAsync()
        {
            return "";
        }

        public async Task AddAsync(Account account)
        {
            return;
        }

        public async Task<Result<Account>> GetAsync(StringId userId, IntId accountId)
        {
            return new Failure("NotImplemented", "NotImplemented");
        }

        public async Task<Result<Account>> GetAsync(IntId accountId)
        {
            return new Failure("NotImplemented", "NotImplemented");
        }

        public async Task UpdateAsync(Account account)
        {
            return;
        }
        }
}
