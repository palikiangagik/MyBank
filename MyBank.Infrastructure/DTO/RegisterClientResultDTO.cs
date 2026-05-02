
using Microsoft.AspNetCore.Identity;

namespace MyBank.Infrastructure.DTO
{
    public record RegisterClientResultDTO
    {
        public required int ClientId { get; init; }
        public required IdentityUser User { get; init; }
    }
}
