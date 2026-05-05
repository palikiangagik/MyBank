using ApiDTO = MyBank.Api.DTO.Client;
using AppDTO = MyBank.Application.DTO.Client;

namespace MyBank.Api.Mappings
{
    public static class ClientMappings
    {
        public static ApiDTO.ClientNameDTO ToApiDTO(this AppDTO.ClientNameDTO appDto)
        {
            return new()
            {
                FirstName = appDto.FirstName,
                LastName = appDto.LastName,
            };
        }

        public static ApiDTO.ClientSummaryDTO ToApiDTO(this AppDTO.ClientSummaryDTO appDto)
        {
            return new()
            {
                Id = appDto.Id,
                Name = appDto.Name.ToApiDTO(),
                TotalBalance = appDto.TotalBalance,
                Accounts = appDto.Accounts.ToApiDTO()
            };
        }
    }
}
