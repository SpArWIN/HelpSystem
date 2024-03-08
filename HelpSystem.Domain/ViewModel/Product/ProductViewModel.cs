using HelpSystem.Domain.ViewModel.Provider;
using HelpSystem.Domain.ViewModel.Warehouse;
using System.ComponentModel.DataAnnotations;

namespace HelpSystem.Domain.ViewModel.Product
{
    public class ProductViewModel
    {
        [Required(ErrorMessage = "Укажите артикул товара")]
        public string InventoryCode { get; set; } = string.Empty;
        [Required(ErrorMessage = "Укажиите наименование товара")]
        public string NameProduct { get; set; } = string.Empty;
        public string? Comments { get; set; }
        [Required(ErrorMessage = "Выберите поставщика")]
        public Guid ProviderID { get; set; }
        [Required(ErrorMessage = "Выберите склад")]
        public Guid WarehouseId { get; set; }
        //Прописываю количество позиций, для первой итерации прохождения по строкам
        // Список поставщиков
        public IEnumerable<ProviderViewModel>? Providers { get; set; }

        // Список складов
        public IEnumerable<WarehouseViewModel>? Warehouses { get; set; }

        public string Quantity { get; set; }

    }
}
