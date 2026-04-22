namespace MyBank.Web.Areas.Web.ViewModels
{
    public class CloseAccountViewModel : BaseViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public decimal Balance { get; set; }
    }
}
