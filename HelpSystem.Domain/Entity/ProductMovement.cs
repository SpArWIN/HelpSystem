using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpSystem.Domain.Entity
{
    //Таблица для перемещения товара по складу
    public class ProductMovement
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; } // ID перемещаемого товара
        public Guid SourceWarehouseId { get; set; } // ID исходного склада
        public Guid DestinationWarehouseId { get; set; } // ID целевого склада
        public DateTime MovementDate { get; set; } // Дата перемещения

    
        public virtual Products Product { get; set; } // Перемещаемый товар
       
    }
}
