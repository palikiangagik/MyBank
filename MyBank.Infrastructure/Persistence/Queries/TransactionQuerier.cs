using CorePrimitives;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MyBank.Application.DTO;
using MyBank.Application.Interfaces;
using MyBank.Domain.Entities;
using System.Data.Common;

namespace MyBank.Infrastructure.Persistence.Queries
{
    public class TransactionQuerier : ITransactionQuerier
    {
        private readonly MyBankDbContext _db;

        public TransactionQuerier(MyBankDbContext db)
        {
            _db = db;
        }


        public async Task<SubList<TransactionHistoryItemDTO>> GetTransactionHistoryAsync(int clientId, PagingParametersDTO pageParameters)
        {            
            const string sqlRows = @"
                    SELECT 
                        T.Id, 
                        T.[Type], 
                        T.CreatedAt, 
                        T.Amount, 

	                    Accounts.Code as AccountCode,

	                    SenderAccounts.Code as SenderAccountCode, 
                        SenderClients.FirstName as SenderFirstName, 
                        SenderClients.LastName as SenderLastName, 

	                    RecipientAccounts.Code as RecipientAccountCode, 
                        RecipientClients.FirstName as RecipientFirstName, 
                        RecipientClients.LastName as RecipientLastName,

                        COUNT(*) OVER() AS TotalCount

                    FROM 
                        Transactions AS T

                    LEFT JOIN Accounts AS Accounts ON Accounts.Id=T.AccountId

                    LEFT JOIN Accounts AS SenderAccounts ON SenderAccounts.Id=T.SenderAccountId
                    LEFT JOIN Clients AS SenderClients ON SenderAccounts.ClientId=SenderClients.Id

                    LEFT JOIN Accounts AS RecipientAccounts ON RecipientAccounts.Id=T.RecipientAccountId
                    LEFT JOIN Clients AS RecipientClients ON RecipientAccounts.ClientId=RecipientClients.Id

                    WHERE 
                        SenderAccounts.ClientId=@ClientId OR
                        RecipientAccounts.ClientId=@ClientId OR
                        Accounts.ClientId=@ClientId

                    ORDER BY T.Id DESC
                    OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY";

            var rows = await _db.Connection.QueryAsync(sqlRows, new
            {
                ClientId = clientId,
                Offset = (pageParameters.Page - 1) * pageParameters.PageSize,
                Limit = pageParameters.PageSize,
            }, _db.Transaction);


            var items = rows.Select(row => new TransactionHistoryItemDTO
            {
                Id = row.Id,
                Type = (TransactionType)row.Type,
                CreatedAt = row.CreatedAt,
                Amount = row.Amount,
                AccountCode = row.AccountCode,
                Sender = row.SenderAccountCode is not null ? new TransactionHistoryItemDTO.Party
                {
                    AccountCode = row.SenderAccountCode,
                    FirstName = row.SenderFirstName,
                    LastName = row.SenderLastName
                } : null,
                Recipient = row.RecipientAccountCode is not null ? new TransactionHistoryItemDTO.Party
                {
                    AccountCode = row.RecipientAccountCode,
                    FirstName = row.RecipientFirstName,
                    LastName = row.RecipientLastName
                } : null
            });

            int totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

            return new SubList<TransactionHistoryItemDTO>(items.ToList(), totalCount);
        }
    }
}
