using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpSystem.Domain.ViewModel.Warehouse
{
    //Тут будет фильтрация по складам, за счёт этой модели выведу список товаров на этом складе
    public class ProductinWarehouseViewModel
    { 
        public string NameProduct { get; set; }
        public string CodeProduct { get; set; }
        public string Provider { get; set; }
        //Количество этого твоара на складе
        public int TotalCountWarehouse { get; set; }
        // Какое количество доступно, то есть не привязано
        public int AvailableCount { get; set; }
    }
}
