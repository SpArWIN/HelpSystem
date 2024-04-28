using HelpSystem.Domain.ViewModel.Warehouse;

namespace HelpSystem.Domain.ViewModel.Report
{
    public class ReportsProductOnWarehouseViewModel
    {
        //Модель, которая будет отвечать за отчёт отображения товаров на складе
        //Включая перемещённые товары
        public ReportsProductOnWarehouseViewModel()
        {
            WarehousesReports = new List<WarehouseReports>();
        }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public List<WarehouseReports> WarehousesReports { get; set; }

    }
}
