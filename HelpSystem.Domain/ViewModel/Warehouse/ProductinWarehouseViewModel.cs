using HelpSystem.Domain.ViewModel.Product;
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
        public int TotalCountWarehouse { get; set; } // Общее количество товаров для группы
        public string NameProduct { get; set; } // Будем по нему группировать
        public IEnumerable<ProductListWarehouse> WhProducts { get; set; } // Список товаров в группе
    }
}
