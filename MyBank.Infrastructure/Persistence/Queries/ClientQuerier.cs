using CorePrimitives;
using Dapper;
using MyBank.Application.DTO.Accounts;
using MyBank.Application.DTO.Client;
using MyBank.Application.DTO.Common;
using MyBank.Application.Interfaces;
using MyBank.Domain.Common;

namespace MyBank.Infrastructure.Persistence.Queries
{
    public class ClientQuerier : IClientQuerier
    {
        private readonly MyBankDbContext _db;

        public ClientQuerier(MyBankDbContext db)
        {
            _db = db;
        }

        public async Task<Result<ClientNameDTO>> GetClientNameAsync(int clientId)
        {
            const string sql = "SELECT FirstName, LastName FROM Clients WHERE Id = @ClientId";
            var result = await _db.Connection.QuerySingleOrDefaultAsync<ClientNameDTO>(
                sql, 
                new { ClientId = clientId }, 
                transaction: _db.Transaction
            );
            if (result is null)
                return Errors.ClientNotFound;
            return result;
        }

        public async Task<Result<ClientSummaryDTO>> GetClientSummaryAsync(int clientId, PagingParametersDTO pageParameters)
        {
            const string sqlClientSummary = @"
                SELECT
                    C.FirstName, 
                    C.LastName, 
                    ISNULL(SUM(A.Balance), 0) AS TotalBalance,
                    COUNT(A.Id) AS AccountCount
                FROM Clients C
                LEFT JOIN Accounts AS A ON C.Id = A.ClientId AND A.IsClosed = 0
                WHERE C.Id = @ClientId
                GROUP BY C.FirstName, C.LastName";

            var summaryResult = await _db.Connection.QuerySingleOrDefaultAsync(sqlClientSummary, 
                new {ClientId = clientId}, 
                transaction: _db.Transaction);

            if (summaryResult is null)
                return Errors.ClientNotFound;

            const string sqlAccounts = @"
                SELECT Id, Code, Balance
                FROM Accounts
                WHERE ClientId=@ClientId AND IsClosed=0
                ORDER BY Id
                OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY";

            var resultAccounts = await _db.Connection.QueryAsync(sqlAccounts, new
            {
                ClientId = clientId,
                Offset = (pageParameters.Page - 1) * pageParameters.PageSize,
                Limit = pageParameters.PageSize
            }, transaction: _db.Transaction);

            return new ClientSummaryDTO
            {
                Id = clientId,
                Name = new ClientNameDTO
                {
                    FirstName = summaryResult.FirstName,
                    LastName = summaryResult.LastName
                },
                TotalBalance = summaryResult.TotalBalance,
                Accounts = new AccountSummaryListDTO
                {
                    TotalCount = summaryResult.AccountCount,
                    Items = resultAccounts.Select(row => new AccountSummaryDTO {
                        Id = (int)row.Id,
                        Code = (string)row.Code,
                        Balance = (decimal)row.Balance
                    }).ToList(),
                }
            };
        }
    }
}
