using Dapper;
using CorePrimitives;
using MyBank.Application.DTO;
using MyBank.Application.Interfaces;

namespace MyBank.Infrastructure.Persistence.Queries
{
    public class ProfileQuerier : IProfileQuerier
    {
        private readonly DbSession _db;

        public ProfileQuerier(DbSession db)
        {
            _db = db;
        }

        public async Task<ProfileSummaryDTO> GetProfileSummaryAsync(string currentUserId, int page, int pageSize)
        {
            if (page < 1 || pageSize <= 0)
                throw new ArgumentException("Page must be >= 1 and PageSize must be > 0.");

            var conn = await _db.GetConnection();

            decimal totalBalance = 0;
            List<ProfileSummaryAccountItemDTO> items = [];

            const string sqlTotalCount = "SELECT COUNT(*) FROM Accounts WHERE UserId = @UserId AND IsClosed=0";
            int totalCount = await conn.ExecuteScalarAsync<int>(sqlTotalCount, new
            {
                UserId = currentUserId
            }, _db.Transaction);


            if (totalCount > 0)
            {
                const string sqlTotalBalance = "SELECT SUM(Balance) FROM Accounts WHERE UserId = @UserId AND IsClosed=0";
                totalBalance = await conn.ExecuteScalarAsync<decimal>(sqlTotalBalance, new
                {
                    UserId = currentUserId
                }, _db.Transaction);

                const string sqlAccounts = @"
                    SELECT Id, Code, Balance
                    FROM Accounts
                    WHERE UserId=@UserId AND IsClosed=0
                    ORDER BY Id
                    OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY";
                var rows = await conn.QueryAsync(sqlAccounts, new
                {
                    UserId = currentUserId,
                    Offset = (page - 1) * pageSize,
                    Limit = pageSize
                }, _db.Transaction);

                items = rows.Select(row => new ProfileSummaryAccountItemDTO(
                    (int)row.Id, 
                    (string)row.Code, 
                    (decimal)row.Balance)
                ).ToList();
            }

            return new ProfileSummaryDTO(
                totalBalance,
                new SubList<ProfileSummaryAccountItemDTO>(items, totalCount)
            );
        }
    }
}
