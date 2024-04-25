using HelpSystem.Domain.ViewModel.Report;
using HelpSystem.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HelpSystem.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IReportService _reportService;
        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ReportWarehouses(DateTime StartTime, DateTime EndTime)
        {
            if (StartTime == default(DateTime) || EndTime == default(DateTime))
            {
                return BadRequest(new { description = "Одна или обе даты являются пустыми" });
            }

            var Response = await _reportService.GetWarehouseReport(StartTime, EndTime);



            if (Response.StatusCode == Domain.Enum.StatusCode.Ok)
            {
                return Ok(new { description = Response.Description, Response.Data });
            }
            return BadRequest(new { description = Response.Description });

        }
        //Метод отвечающий за получения информации о товарах, котоыре прикреплены к этому пользователю
        [HttpPost]
        public async Task<IActionResult> ReportUserProducts(Guid UserId)
        {
            var Response = await _reportService.GetUserReports(UserId);
            if (Response.StatusCode == Domain.Enum.StatusCode.Ok)
            {
                return Json( new {data = Response.Data, description = Response.Description});
            }

            return Json(Response.Description);
        }

        [HttpGet]
        //Метод отвечающий за получение всех пользователей с их прикреплёнными товарами
        public async Task<IActionResult> ReportUsersProducts()
        {
            var Response = await _reportService.GetUsersReports();
            if (Response.StatusCode == Domain.Enum.StatusCode.Ok)
            {
                return Json(new { data = Response.Data, description = Response.Description });
            }

            return Json(Response.Description);
        }

    
    }
}
