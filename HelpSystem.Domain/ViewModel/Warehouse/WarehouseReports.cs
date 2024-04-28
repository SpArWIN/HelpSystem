using HelpSystem.Domain.ViewModel.Product;

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
