using CorePrimitives;
using Dapper;
using MyBank.Application.DTO;
using MyBank.Application.Interfaces;
using MyBank.Domain.Common;
using System.Security;

namespace MyBank.Infrastructure.Persistence.Queries
{
    public class AccountQuerier : IAccountQuerier
    {
        private readonly UnitOfWork _uow;

        public AccountQuerier(UnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<Result<AccountSummaryDTO>> GetAccountSummaryAsync(string currentUserId, int accountId)
        {
            var conn = await _uow.GetConnection();

            const string sql = @"SELECT Id, Code, Balance FROM Accounts 
                WHERE UserId=@CurrentUserId AND Id=@AccoundId";

            var row = await conn.QuerySingleOrDefaultAsync<dynamic>(sql, new {
                CurrentUserId = currentUserId,
                AccoundId = accountId
            }, _uow.Transaction);

            if (row == null)
                return Failures.AccountNotFound;

            return new AccountSummaryDTO(row.Id, row.Code, row.Balance);
        }

        public async Task<Result<SubList<DestinationAccountDTO>>> GetDestinationAccountListAsync(string currentUserId,
            int page, int pageSize)
        {
            var conn = await _uow.GetConnection();

            const string sqlCount = "SELECT COUNT(*) FROM Accounts";
            int totalCount = await conn.ExecuteScalarAsync<int>(sqlCount, _uow.Transaction);

            const string sqlRows = @"
                SELECT A.Id, U.UserName, A.Code FROM Accounts AS A 
                LEFT JOIN AspNetUsers AS U ON A.UserId=U.Id 
                ORDER BY A.Id
                OFFSET @Offset ROWS
                FETCH NEXT @Limit ROWS ONLY";
            var rows = await conn.QueryAsync(sqlRows, new { Offset = (page - 1) * pageSize, Limit = pageSize }, _uow.Transaction);
            var items = rows.Select(row => new DestinationAccountDTO(row.Id, row.UserName, row.Code)).ToList();

            return new SubList<DestinationAccountDTO>(items, totalCount);
        }
    }
}
