using CorePrimitives;
using MyBank.Application.DTO;

namespace MyBank.Application.Interfaces
{
    public interface IProfileQuerier
    {
        public Task<Result<ProfileSummaryDTO>> GetProfileSummaryAsync(string currentUserId, int page, int pageSize);
    }
}
