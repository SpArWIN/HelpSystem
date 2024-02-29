using HelpSystem.Domain.ViewModel.Statment;
using HelpSystem.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace HelpSystem.Controllers
{
    public class StatmentController : Controller
    {
        private readonly IStatmentIService _statmentIService;

        public StatmentController(IStatmentIService statment)
        {
            _statmentIService = statment;
        }
        [HttpGet]
        public IActionResult CreateStatment() => View();

        [HttpPost]
        public async Task<IActionResult> CreateStatment(StatmentViewModel model)
        {

            var currentUser = HttpContext.User;
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                var errorDescription = string.Join(",", errors);
                return BadRequest(new { description = errorDescription });
            }
            var userIdClaim = currentUser.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                //Завтра доделай это
                var Response = await _statmentIService.CreateStatment(model, userId);

                if (Response.StatusCode == Domain.Enum.StatusCode.Ok)
                {
                    return Ok(new { description = Response.Description });

                }
                return BadRequest(new { description = Response.Description });
            }

            return View();

        }
        //Получение списка заявок конкретного пользователя - именно заявок. 
        //Метод возвращает Json В таблицу, нужен отдельный метод, который бы возвращал данные в частичное предсставление


        public async Task<IActionResult> GetUserStatment()
        {
            //Нужно как то разобраться с пагинацией
            //Пагинация
            //var Start = Request.Form["start"].FirstOrDefault();

            //var lengths = Request.Form["length"].FirstOrDefault();
            //var PageSize = lengths != null ? Convert.ToInt32(lengths) : 0;

            //var Skip = Start != null ? Convert.ToInt32(Start) : 0;

            //Page page = new Page()
            //{
            //    PageSize = PageSize,
            //    Skip = Skip
            //};

            var currentUser = HttpContext.User;
            var userIdClaim = currentUser.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                var Response = await _statmentIService.GetStatment(userId);
               
                    //доделать заявки и тут разобраться

                    return Json(new { data = Response.Data });
                
            }

            return Json(new { /*recordsFiltered = Response.Total, recordsTotal = Response.Total,*/ data = "Нет заявок" });

        }

        [HttpGet]
        public IActionResult GetStatment() => View();
        //По поему это Post
        [HttpPost]
        public async Task<IActionResult> GetStatmentsData()
        {
            var response = await _statmentIService.GetAllStatments();

            return Json(new { data = response.Data });

        }
        //Метод обновления статуса заявки при её просмотре администратором
        [HttpPost]
        public async Task<IActionResult> UpdateStatusStat(Guid id, int NewStatus)
        {
      
            var ResUpdate = await _statmentIService.UpdateStatusStat(id, NewStatus);
            if (ResUpdate.StatusCode == Domain.Enum.StatusCode.Ok)
            {
                return Json(new { data = ResUpdate.Data });
            }

            return Json(new {data = ResUpdate.Description});
        }
        //Этот метод действия отвечает за получения именно заявки 
        public async Task<IActionResult> GetStatUser(Guid id)
        {
            var Res = await _statmentIService.GetStat(id);
            if (Res.StatusCode == Domain.Enum.StatusCode.Ok)
            {
                return PartialView("_PartialStatment", Res.Data);
            }

            return PartialView("_PartialStatment");
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}
