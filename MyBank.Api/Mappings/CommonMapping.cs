using ApiDTO = MyBank.Api.DTO.Common;
using AppDTO = MyBank.Application.DTO.Common;

namespace MyBank.Api.Mappings
{
    public static class CommonMapping
    {
        public static AppDTO.PagingParametersDTO ToAppDTO(this ApiDTO.PagingParametersDTO apiDto)
        {
            return new(apiDto.Page, apiDto.PageSize);
        }
    }
}
