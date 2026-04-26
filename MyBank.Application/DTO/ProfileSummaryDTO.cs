using CorePrimitives;

namespace MyBank.Application.DTO
{
    public record ProfileSummaryAccountItemDTO(int Id, string Code, decimal Balance);
    public record ProfileSummaryDTO(decimal TotalBalance, SubList<ProfileSummaryAccountItemDTO> AccountList);
}
