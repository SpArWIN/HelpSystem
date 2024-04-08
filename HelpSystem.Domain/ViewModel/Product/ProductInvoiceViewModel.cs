using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpSystem.Domain.ViewModel.Product
{
    public class ProductInvoiceViewModel
    {
        //Класс для отображения товаров в накладной 
        public string NumberDoc { get; set; }
        public string NameProduct { get; set; }
        public string CodeProduct { get; set; }
        public string Warehouse { get; set; }
        public string Provider { get; set; }
       

    }
}
