using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyBank.Portal.Areas.Portal.ViewModels
{    
    public class DepositAndWithdrawalViewModel : BaseViewModel
    {
        public List<SelectListItem> Accounts { get; set; }
        [Display(Name = "Account")]
        public int Account { get; set; }
        [Display(Name = "Amount")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }
    }
}
