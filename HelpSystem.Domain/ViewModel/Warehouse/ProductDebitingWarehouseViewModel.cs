using HelpSystem.Domain.ViewModel.Product;

namespace HelpSystem.Domain.ViewModel.Warehouse
{
    //Модель для отображения товаров, находищхся под списанием 
    public class ProductDebitingWarehouseViewModel
    {
        public string ProductName { get; set; } //группировка будет по нему 

        public IEnumerable<ProductDebitingViewModel> Whproduct { get; set; }
        public int TotalCount { get; set; } // Общее количество


    }
}
