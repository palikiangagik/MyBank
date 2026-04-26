using CorePrimitives;
using MyBank.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBank.Application.Interfaces
{
    public interface IAccountQuerier
    {
        public Task<Result<AccountSummaryDTO>> GetAccountSummaryAsync(string currentUserId, int accountId);
        public Task<SubList<AccountSummaryDTO>> GetUserAccountListAsync(string currentUserId, int page, int pageSize);
        public Task<SubList<DestinationAccountDTO>> GetDestinationAccountListAsync(string currentUserId, 
            int page, int pageSize);
    }
}
