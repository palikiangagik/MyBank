namespace MyBank.Application.DTO.Common
{
    public record PagingParametersDTO
    {
        public PagingParametersDTO(int page, int pageSize)
        {
            if (page < 1)
                throw new ArgumentException("Page must be >= 1.", nameof(page));
            if (pageSize <= 0)
                throw new ArgumentException("PageSize must be > 0.", nameof(pageSize));
            Page = page;
            PageSize = pageSize;
        }

        public int Page { get; init; }
        public int PageSize { get; init; }
    }
}
