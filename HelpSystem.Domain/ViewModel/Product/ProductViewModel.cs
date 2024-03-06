using HelpSystem.Domain.ViewModel.Provider;
using HelpSystem.Domain.ViewModel.Warehouse;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpSystem.Domain.ViewModel.Product
{
    public class ProductViewModel
    {
        public string InventoryCode { get; set; } = string.Empty; 
       
        public string NameProduct { get; set; } = string.Empty;
        public string? Comments { get; set; }
        [Required(ErrorMessage = "Выберите поставщика")]
        public Guid ProviderID { get; set; }
        [Required(ErrorMessage = "Выберите склад")]
        public Guid WarehouseId { get; set; }
        //Прописываю количество позиций, для первой итерации прохождения по строкам
        // Список поставщиков
        public IEnumerable<ProviderViewModel> Providers { get; set; }

        // Список складов
        public IEnumerable<WarehouseViewModel> Warehouses { get; set; }



    }
}
