using CorePrimitives;
using MyBank.Application.DTO;

namespace MyBank.Application.Interfaces
{
    public interface IProfileQuerier
    {
        public Task<ProfileSummaryDTO> GetProfileSummaryAsync(string currentUserId, int page, int pageSize);
    }
}
