using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyBank.Web.Areas.Web.ViewModels
{
    public class ProfileAccountViewItem : BaseViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public decimal Balance { get; set; }
    }

    public class ProfileViewModel
    {        
        public string UserName { get; set; }
        public decimal Balance { get; set; }
        public List<ProfileAccountViewItem> Accounts { get; set; }
    }
}
