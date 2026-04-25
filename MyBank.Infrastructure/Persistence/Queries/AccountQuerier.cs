using CorePrimitives;
using MyBank.Application.DTO;
using MyBank.Application.Interfaces;

namespace MyBank.Infrastructure.Persistence.Queries
{
    public class AccountQuerier : IAccountQuerier
    {
        public async Task<Result<AccountSummaryDTO>> GetAccountSummaryAsync(string currentUserId, string accountId)
        {
            return new Failure("NotImplemented", "NotImplemented");
        }

        public async Task<Result<SubList<DestinationAccountDTO>>> GetDestinationAccountListAsync(string currentUserId,
            int page, int pageSize)
        {
            return new Failure("NotImplemented", "NotImplemented");
        }
    }
}
