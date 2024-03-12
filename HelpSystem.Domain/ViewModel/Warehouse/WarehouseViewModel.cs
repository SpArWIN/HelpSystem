using System.ComponentModel.DataAnnotations;

namespace HelpSystem.Domain.ViewModel.Warehouse
{
    public class WarehouseViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Укажите имя склада")]
        public string WarehouseName { get; set; }
        public  int ? TotalCountWarehouse {get; set; }

    }
}
