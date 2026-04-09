using System.Collections.Generic;

namespace MyBank.Portal.Areas.Portal.ViewModels
{
    public class AccountViewModel
    {
        public int Id { get; set; }
        public decimal Balance { get; set; }
    }

    public class ProfileViewModel
    {
        public string UserName { get; set; }
        public decimal Balance { get; set; }
        public List<AccountViewModel> Accounts { get; set; }
    }
}
