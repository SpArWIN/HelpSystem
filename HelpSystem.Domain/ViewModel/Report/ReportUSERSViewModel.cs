using HelpSystem.Domain.ViewModel.Users;

namespace HelpSystem.Domain.ViewModel.Report
{
    public class ReportUSERSViewModel
    {
        public ReportUSERSViewModel()
        {
            Users = new List<UserInfoViewModel>();
        }
        //Подсчёт всех товаров прикреплённых к юзверю


        public List<UserInfoViewModel>? Users { get; set; }
    }
}
