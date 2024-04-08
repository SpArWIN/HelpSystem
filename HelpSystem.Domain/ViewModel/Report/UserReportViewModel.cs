using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpSystem.Domain.ViewModel.Report
{
    public class UserReportViewModel
    {
        //Это для одного пользователя
        public string? Name { get; set; }
        public string? SurName { get; set; }
        public string ? LastName { get; set; }

        public string ProductName { get; set; }
        public string Code { get; set; }
        public int Quantity { get; set; }
        public string? FullName
        {
            get { return $"{LastName} {Name} {SurName}"; }
            set { }
        }
        public int TotalCount { get; set; }
    }
}
