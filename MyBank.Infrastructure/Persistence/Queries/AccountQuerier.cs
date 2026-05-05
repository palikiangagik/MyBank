using CorePrimitives;
using Dapper;
using MyBank.Application.DTO.Accounts;
using MyBank.Application.DTO.Client;
using MyBank.Application.DTO.Common;
using MyBank.Application.Interfaces;
using MyBank.Domain.Common;
namespace MyBank.Infrastructure.Persistence.Queries
{
    public class AccountQuerier : IAccountQuerier
    {
        private readonly MyBankDbContext _db;

        public AccountQuerier(MyBankDbContext db)
        {
            _db = db;
        }

        public async Task<Result<AccountSummaryDTO>> GetAccountSummaryAsync(int clientId, int accountId)
        {
            const string sql = @"SELECT Id, Code, Balance FROM Accounts 
                WHERE ClientId=@ClientId AND Id=@AccountId AND IsClosed=0";

            var row = await _db.Connection.QuerySingleOrDefaultAsync<dynamic>(sql, new {
                ClientId = clientId,
                AccountId = accountId
            }, transaction: _db.Transaction);

            if (row == null)
                return Errors.AccountNotFound;

            return new AccountSummaryDTO{
                Id = row.Id, 
                Code = row.Code,
                Balance = row.Balance
            };
        }

        public async Task<AccountSummaryListDTO> GetClientAccountListAsync(int clientId, PagingParametersDTO pageParameters)
        {
            const string sqlCount = "SELECT COUNT(*) FROM Accounts WHERE ClientId=@ClientId AND IsClosed=0";
            int totalCount = await _db.Connection.ExecuteScalarAsync<int>(sqlCount, new {
                ClientId = clientId
            }, transaction: _db.Transaction);

            const string sqlRows = @"
                SELECT Id, Code, Balance FROM Accounts 
                WHERE ClientId=@ClientId AND IsClosed=0
                ORDER BY Id
                OFFSET @Offset ROWS
                FETCH NEXT @Limit ROWS ONLY
            ";

            var rows = await _db.Connection.QueryAsync(sqlRows, new {
                ClientId = clientId,
                Offset = (pageParameters.Page - 1) * pageParameters.PageSize, 
                Limit = pageParameters.PageSize
            }, transaction: _db.Transaction);

            return new AccountSummaryListDTO {
                TotalCount = totalCount,
                Items = rows.Select(row => new AccountSummaryDTO
                {
                    Id = row.Id,
                    Code = row.Code,
                    Balance = row.Balance
                }).ToList(),
            };
        }

        public async Task<DestinationAccountListDTO> GetDestinationAccountListAsync(int clientId, PagingParametersDTO pageParameters)
        {
            const string sqlCount = "SELECT COUNT(*) FROM Accounts WHERE ClientId<>@ClientId AND IsClosed=0";
            int totalCount = await _db.Connection.ExecuteScalarAsync<int>(sqlCount, new { ClientId = clientId }, 
                transaction: _db.Transaction);

            const string sqlRows = @"
                SELECT A.Id, C.FirstName, C.LastName, A.Code FROM Accounts AS A 
                LEFT JOIN Clients AS C ON A.ClientId=C.Id 
                WHERE A.ClientId<>@ClientId AND A.IsClosed=0
                ORDER BY A.Id
                OFFSET @Offset ROWS
                FETCH NEXT @Limit ROWS ONLY";

            var rows = await _db.Connection.QueryAsync(sqlRows, new { 
                ClientId = clientId,
                Offset = (pageParameters.Page - 1) * pageParameters.PageSize,
                Limit = pageParameters.PageSize,
            }, transaction: _db.Transaction);


            return new DestinationAccountListDTO
            {
                TotalCount = totalCount,
                Items = rows.Select(row => new DestinationAccountDTO
                {
                    Id = row.Id,
                    Code = row.Code,
                    Name = new ClientNameDTO
                    {
                        FirstName = row.FirstName,
                        LastName = row.LastName
                    }
                }).ToList()
            };
        }
    }
}
