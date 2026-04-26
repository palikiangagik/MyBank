using CorePrimitives;
using MyBank.Application.DTO;
using MyBank.Application.Interfaces;

namespace MyBank.Application.UseCases
{
    public class ProfileUseCases
    {
        private readonly IProfileQuerier _profileQuerer;

        public ProfileUseCases(IProfileQuerier profileQuerer)
        {
            _profileQuerer = profileQuerer;
        }

        public Task<ProfileSummaryDTO> GetProfileSummaryAsync(string currentUserId, int page, int pageSize) =>
            _profileQuerer.GetProfileSummaryAsync(currentUserId, page, pageSize);


    }
}
