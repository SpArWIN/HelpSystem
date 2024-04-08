using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
