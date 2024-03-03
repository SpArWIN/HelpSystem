using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpSystem.Domain.Entity
{
    public class Products
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Mass { get; set; } = string.Empty;
        //public string Type {get;set;} -если будет нужно, то добавим товару тип
        public string ? Comments { get; set; }
        public DateTime DeliveryDate { get; set; }
        //Внешний ключ поставщика
        //public Guid ProviderId { get; set; }
        public virtual Provider Provider { get; set; }
        ////Внешний ключ Склада
        //public Guid WarehouseID { get; set; }
        public virtual Warehouse Warehouse { get; set; }
       // Внешний ключ
        public Guid? UserId { get; set; }
        public virtual User?  User { get; set; }

    }
}
