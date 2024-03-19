using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpSystem.Domain.ViewModel.Product
{
    public class BindingProductWarehouse
    {
        public Guid UserId { get; set; }
        public string ProductName { get; set; }
        public string InventoryCode { get; set; }
        //Тут мы передаем количество товара, которо нужно привязать
        /*
         * И соответственно логика обработки будет, если указано недопустимое количество,
         * то будет возврат
         */
        public int CountBinding { get; set; }
    }
}
