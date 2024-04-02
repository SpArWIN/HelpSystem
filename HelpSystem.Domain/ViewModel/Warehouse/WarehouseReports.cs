using HelpSystem.Domain.ViewModel.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpSystem.Domain.ViewModel.Warehouse
{
    public class WarehouseReports
    {
        //Класс для отчёта по складам
        public string WarehouseName { get; set; }
        public List<ProductsInfo> ProductsInfo { get; set; }
        public int TotalQuantity { get; set; }
    }
}
