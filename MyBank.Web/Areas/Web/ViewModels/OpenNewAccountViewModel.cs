using System.ComponentModel.DataAnnotations;

namespace MyBank.Web.Areas.Web.ViewModels
{
    public class OpenNewAccountViewModel : BaseViewModel
    {
        [Required]
        [Display(Name = "Amount")]
        [Range(0, double.MaxValue, ErrorMessage = "Amount must be positive.")]
        public decimal Amount { get; set; }
    }
}
