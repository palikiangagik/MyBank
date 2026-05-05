using ApiClientDTO = MyBank.Api.DTO.Client;
using ApiDTO = MyBank.Api.DTO.Accounts;
using AppDTO = MyBank.Application.DTO.Accounts;

namespace MyBank.Api.Mappings
{
    public static class AccountsMappings
    {
        public static ApiDTO.AccountSummaryDTO ToApiDTO(this AppDTO.AccountSummaryDTO appDto)
        {
            return new()
            {
                Id = appDto.Id,
                Code = appDto.Code,
                Balance = appDto.Balance
            };
        }

        public static ApiDTO.AccountSummaryListDTO ToApiDTO(this AppDTO.AccountSummaryListDTO appDto)
        {
            return new()
            {
                TotalCount = appDto.TotalCount,
                Items = appDto.Items.Select(i => i.ToApiDTO()).ToList()
            };
        }

        public static ApiDTO.DestinationAccountDTO ToApiDTO(this AppDTO.DestinationAccountDTO appDto)
        {
            return new()
            {
                Id = appDto.Id,
                Code = appDto.Code,
                Name = new ApiClientDTO.ClientNameDTO
                {
                    FirstName = appDto.Name.FirstName,
                    LastName = appDto.Name.LastName
                }
            };            
        }

        public static ApiDTO.DestinationAccountListDTO ToApiDTO(this AppDTO.DestinationAccountListDTO appDto)
        {
            return new()
            {
                TotalCount = appDto.TotalCount,
                Items = appDto.Items.Select(i => i.ToApiDTO()).ToList()
            };
        }
    }
}
