using ApiDTO = MyBank.Api.DTO.Auth;
using InfraDTO = MyBank.Infrastructure.DTO.Auth;

namespace MyBank.Api.Mappings
{
    public static class AuthMappings
    {
        public static InfraDTO.RegisterClientDTO ToInfraDTO(this ApiDTO.RegisterRequestDTO apiDto)
        {
            return new()
            {
                FirstName = apiDto.FirstName,
                LastName = apiDto.LastName,
                Email = apiDto.Email,
                Password = apiDto.Password
            };
        }

        public static InfraDTO.LoginClientDTO ToInfraDTO(this ApiDTO.LoginRequestDTO apiDto)
        {
            return new()
            {
                Email = apiDto.Email,
                Password = apiDto.Password,
                RememberMe = apiDto.RememberMe
            };
        }
    }
}
