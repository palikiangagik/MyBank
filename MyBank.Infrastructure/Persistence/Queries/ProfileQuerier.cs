using CorePrimitives;
using MyBank.Application.DTO;
using MyBank.Application.Interfaces;

namespace MyBank.Infrastructure.Persistence.Queries
{
    public class ProfileQuerier : IProfileQuerier
    {
        public async Task<Result<ProfileSummaryDTO>> GetProfileSummaryAsync(string currentUserId, int page, int pageSize)
        {
            return new Failure("NotImplemented", "NotImplemented");
        }
    }
}
