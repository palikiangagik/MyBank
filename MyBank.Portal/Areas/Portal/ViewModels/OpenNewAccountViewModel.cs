using System.ComponentModel.DataAnnotations;

namespace MyBank.Portal.Areas.Portal.ViewModels
{
    public class OpenNewAccountViewModel
    {
        [Required]
        [Display(Name = "Amount")]
        [Range(0, double.MaxValue, ErrorMessage = "Amount must be positive.")]
        public decimal Amount { get; set; }
    }
}
