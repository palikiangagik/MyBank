using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyBank.Web.Areas.Web.ViewModels
{
    public class MoneyTransferViewModel : BaseViewModel
    {
        public List<SelectListItem> FromAccounts { get; set; }
        public List<SelectListItem> ToAccounts { get; set; }

        [Required]
        [Display(Name = "From Account")]
        public int FromAccount { get; set; }
        [Required]
        [Display(Name = "To Account")]
        public int ToAccount { get; set; }
        [Required]
        [Display(Name = "Amount")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }
    }
}
