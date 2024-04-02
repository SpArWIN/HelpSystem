using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Report;

namespace HelpSystem.Service.Interfaces
{
    public interface IReportService
    {
        //Получения отчётности о товарах, прибывших на склад по периоду времени
        Task<IBaseResponse<ReportsProductOnWarehouseViewModel>> GetWarehouseReport(DateTime startDate, DateTime endDate);
    }
}
