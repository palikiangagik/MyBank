using CorePrimitives;

namespace MyBank.Application.DTO
{        
    public record ClientSummaryDTO
    {
        public record ClientName
        {
            public required string FirstName { get; init; }
            public required string LastName { get; init; }
        }

        public record AccountItem
        {
            public required int Id { get; init; }
            public required string Code { get; init; }
            public required decimal Balance { get; init; }
        }

        public required int Id { get; init; }
        public required ClientName Name { get; init; }
        public required decimal TotalBalance { get; init; }
        public required SubList<AccountItem> AccountList { get; init; }   
    }           
}
