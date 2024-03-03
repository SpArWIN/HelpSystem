using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpSystem.Domain.ViewModel.Warehouse
{
    public class WarehouseViewModel
    {

        [Required(ErrorMessage = "Укажите имя склада")]
        public string WarehouseName { get; set; }
    }
}
