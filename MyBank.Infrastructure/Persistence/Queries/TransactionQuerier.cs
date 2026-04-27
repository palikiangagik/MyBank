using Dapper;
using CorePrimitives;
using MyBank.Application.DTO;
using MyBank.Application.Interfaces;
using MyBank.Domain.Entities;

namespace MyBank.Infrastructure.Persistence.Queries
{
    public class TransactionQuerier : ITransactionQuerier
    {
        private readonly DbSession _db;

        public TransactionQuerier(DbSession db)
        {
            _db = db;
        }


        public async Task<SubList<TransactionHistoryItemDTO>> GetTransactionHistoryAsync(
           string currentUserId, int page, int pageSize)
        {
            if (page < 1 || pageSize <= 0)
                throw new ArgumentException("Page must be >= 1 and PageSize must be > 0.");

            var conn = await _db.GetConnection();

            const string sqlCount = @"
                    SELECT COUNT(*) FROM Transactions AS T

                    LEFT JOIN Accounts AS SenderAccounts ON SenderAccounts.Id=T.SenderAccountId
                    LEFT JOIN Accounts AS RecipientAccounts ON RecipientAccounts.Id=T.RecipientAccountId
                    LEFT JOIN Accounts AS Accounts ON Accounts.Id=T.AccountId

                    WHERE SenderAccounts.UserId=@CurrentUserId
                       OR RecipientAccounts.UserId=@CurrentUserId
                       OR Accounts.UserId=@CurrentUserId";
            int totalCount = await conn.ExecuteScalarAsync<int>(sqlCount, new 
            { 
                CurrentUserId = currentUserId
            }, _db.Transaction);
            

            const string sqlRows = @"
                    SELECT T.Id, T.[Type], T.CreatedAt, T.Amount, 
	                    T.SenderAccountId, SenderAccounts.Code as SenderAccountCode, SenderUsers.UserName as SenderUserName, 
	                    T.RecipientAccountId, RecipientAccounts.Code as RecipientAccountCode, RecipientUsers.UserName as RecipientUserName,
	                    T.AccountId, Accounts.Code as AccountCode, AcountUsers.UserName as AccountUserName
                    FROM Transactions AS T


                    LEFT JOIN Accounts AS SenderAccounts ON SenderAccounts.Id=T.SenderAccountId
                    LEFT JOIN AspNetUsers AS SenderUsers ON SenderAccounts.UserId=SenderUsers.Id

                    LEFT JOIN Accounts AS RecipientAccounts ON RecipientAccounts.Id=T.RecipientAccountId
                    LEFT JOIN AspNetUsers AS RecipientUsers ON RecipientAccounts.UserId=RecipientUsers.Id

                    LEFT JOIN Accounts AS Accounts ON Accounts.Id=T.AccountId
                    LEFT JOIN AspNetUsers AS AcountUsers ON Accounts.UserId=AcountUsers.Id

                    WHERE SenderAccounts.UserId=@CurrentUserId
                       OR RecipientAccounts.UserId=@CurrentUserId
                       OR Accounts.UserId=@CurrentUserId
                    ORDER BY T.Id DESC
                    OFFSET @Offset ROWS
                    FETCH NEXT @Limit ROWS ONLY";

            var rows = await conn.QueryAsync(sqlRows, new
            {
                CurrentUserId = currentUserId,
                Offset = (page - 1) * pageSize,
                Limit = pageSize
            }, _db.Transaction);
                        

            var items = rows.Select(row => new TransactionHistoryItemDTO
            {
                Id = row.Id,
                Type = (TransactionType)row.Type,
                CreatedAt = row.CreatedAt,
                Amount = row.Amount,
                AccountCode = row.AccountCode,
                SenderAccountCode = row.SenderAccountCode,
                SenderUserName = row.SenderUserName,
                RecipientAccountCode = row.RecipientAccountCode,
                RecipientUserName = row.RecipientUserName
            }).ToList();

            return new SubList<TransactionHistoryItemDTO>(items, totalCount);
        }
    }
}
