using HelpSystem.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpSystem.Domain.ViewModel.Report
{
    public class ReportsProductOnWarehouseViewModel
    {
        //Модель, которая будет отвечать за отчёт отображения товаров на складе
        //Включая перемещённые товары

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
       public string Warehouse { get; set; }
       //Перемещённый товар по складу
       public int MovedProducts { get; set; }
       public List<Products> Products { get; set; }

    }
}
