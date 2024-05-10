using HelpSystem.Domain.ViewModel.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpSystem.Domain.ViewModel.Report
{
    //Класс для списанных товаров
    public class ReportDebitingProduct
    {
        public string WarehouseName { get; set; } = string.Empty;
        public int TotalCount { get; set; }
        public List<ProductDebitingViewModel> WhDebitingProduct { get; set; } 
    }
}
