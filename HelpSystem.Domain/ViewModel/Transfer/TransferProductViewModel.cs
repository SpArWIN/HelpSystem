using HelpSystem.Domain.ViewModel.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpSystem.Domain.ViewModel.Transfer
{
    //Будет использоваться для окна с перемещением, точнее для отображения товара 
    public class TransferProductViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public IEnumerable<Entity.Warehouse>? Warehouses { get; set; }
        public Guid? DestinationWarehouseId { get; set; } // Склад, на который был перемещен товар
    }
}
