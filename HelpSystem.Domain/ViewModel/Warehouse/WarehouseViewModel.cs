using System.ComponentModel.DataAnnotations;

namespace HelpSystem.Domain.ViewModel.Warehouse
{
    public class WarehouseViewModel
    {

        [Required(ErrorMessage = "Укажите имя склада")]
        public string WarehouseName { get; set; }
    }
}
