using Dapper;
using CorePrimitives;
using MyBank.Application.DTO;
using MyBank.Application.Interfaces;

namespace MyBank.Infrastructure.Persistence.Queries
{
    public class ProfileQuerier : IProfileQuerier
    {
        private readonly UnitOfWork _uow;

        public ProfileQuerier(UnitOfWork unitOfWork)
        {
            _uow = unitOfWork;
        }

        public async Task<ProfileSummaryDTO> GetProfileSummaryAsync(string currentUserId, int page, int pageSize)
        {
            var conn = await _uow.GetConnection();

            decimal totalBalance = 0;
            List<ProfileSummaryAccountItemDTO> items = [];

            const string sqlTotalCount = "SELECT COUNT(*) FROM Accounts WHERE UserId = @UserId";
            int totalCount = await conn.ExecuteScalarAsync<int>(sqlTotalCount, new
            {
                UserId = currentUserId
            }, _uow.Transaction);


            if (totalCount > 0)
            {
                const string sqlTotalBalance = "SELECT SUM(Balance) FROM Accounts WHERE UserId = @UserId";
                totalBalance = await conn.ExecuteScalarAsync<decimal>(sqlTotalBalance, new
                {
                    UserId = currentUserId
                }, _uow.Transaction);



                const string sqlAccounts = @"
                    SELECT Id, Code, Balance
                    FROM Accounts
                    WHERE UserId=@UserId
                    ORDER BY Id
                    OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY";
                var rows = await conn.QueryAsync(sqlAccounts, new
                {
                    UserId = currentUserId,
                    Offset = (page - 1) * pageSize,
                    Limit = pageSize
                }, _uow.Transaction);

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
