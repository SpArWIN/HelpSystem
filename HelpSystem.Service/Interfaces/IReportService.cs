using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Report;

namespace HelpSystem.Service.Interfaces
{
    public interface IReportService
    {
        //Получения отчётности о товарах, прибывших на склад по периоду времени
        Task<IBaseResponse<ReportsProductOnWarehouseViewModel>> GetWarehouseReport(DateTime startDate, DateTime endDate);
        /// <summary>
        /// Получение информации о прикреплённых товарах за конкретным пользователем
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        Task<BaseResponse<IEnumerable<UserReportViewModel>>> GetUserReports(Guid userid);
        /// <summary>
        /// Получение информации о прикреплённых товаров всех юзверей
        /// </summary>
        /// <returns></returns>
        Task<IBaseResponse<ReportUSERSViewModel>> GetUsersReports();

    }
}
