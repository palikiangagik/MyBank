using System.ComponentModel.DataAnnotations;

namespace MyBank.Api.DTO
{
    public record PagingParametersDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than zero.")]
        public int Page { get; init; } = 1;
        [Range(1, 500, ErrorMessage = "Page size must be greater than zero and less or equal than 500.")]
        public int PageSize { get; init; } = 10;
    }
}
