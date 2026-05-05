using MyBank.Domain.Entities;
using ApiDTO = MyBank.Api.DTO.Transactions;
using AppDTO = MyBank.Application.DTO.Transactions;

namespace MyBank.Api.Mappings
{
    public static class TransactionsMapping
    {
        public static string ToApiDTOType(this TransactionType transType)
        {
            return transType switch
            {
                TransactionType.Deposit => "deposit",
                TransactionType.Withdrawal => "withdrawal",
                TransactionType.Transfer => "transfer",
                _ => throw new ArgumentOutOfRangeException(nameof(transType), transType, "Unknown transaction type encountered.")
            };
        }

        public static ApiDTO.DepositTransactionDTO ToApiDTO(this AppDTO.DepositTransactionDTO appDto)
        {
            return new()
            {
                CreatedAt = appDto.CreatedAt,
                Amount = appDto.Amount,
                AccountCode = appDto.AccountCode
            };
        }

        public static ApiDTO.WithdrawalTransactionDTO ToApiDTO(this AppDTO.WithdrawalTransactionDTO appDto)
        {
            return new()
            {
                CreatedAt = appDto.CreatedAt,
                Amount = appDto.Amount,
                AccountCode = appDto.AccountCode
            };
        }

        public static ApiDTO.TransferTransactionDTO ToApiDTO(this AppDTO.TransferTransactionDTO appDto)
        {
            return new()
            {
                CreatedAt = appDto.CreatedAt,
                Amount = appDto.Amount,
                SenderAccountCode = appDto.SenderAccountCode,
                RecipientAccountCode = appDto.RecipientAccountCode
            };
        }

        public static ApiDTO.TransferPartyDTO ToApiDTO(this AppDTO.TransferPartyDTO appDto)
        {
            return new()
            {
                Name = appDto.Name.ToApiDTO(),
                AccountCode = appDto.AccountCode
            };
        }

        public static ApiDTO.TransactionHistoryItemDTO ToApiDTO(this AppDTO.TransactionHistoryItemDTO appDto)
        {
            return new()
            {
                Id = appDto.Id,
                Type = appDto.Type.ToApiDTOType(),
                CreatedAt = appDto.CreatedAt,
                Amount = appDto.Amount,
                AccountCode = appDto.AccountCode,
                Sender = appDto.Sender?.ToApiDTO(),
                Recipient = appDto.Recipient?.ToApiDTO(),
            };
        }

        public static ApiDTO.TransactionHistoryListDTO ToApiDTO(this AppDTO.TransactionHistoryListDTO appDto)
        {
            return new()
            {
                TotalCount = appDto.TotalCount,
                Items = appDto.Items.Select(i => i.ToApiDTO()).ToList()
            };
        }


    }
}
