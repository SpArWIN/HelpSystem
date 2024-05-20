using HelpSystem.Domain.ViewModel.Transfer;
using HelpSystem.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HelpSystem.Controllers
{
    public class TransferController : Controller
    {
        private readonly ITransferService _transferService;

        public TransferController(ITransferService transfer)
        {
            _transferService = transfer;
        }
        [HttpPost]
        //Метод добавление записи о перемещени товара
        public async Task<IActionResult> AddTransfer(List<TransferViewModel> model)
        {
            var Response = await _transferService.AddTransferService(model);
            if (Response.StatusCode == Domain.Enum.StatusCode.Ok)
            {
                return Ok(new { data = Response.Data, description = Response.Description });
            }

            return BadRequest(new { description = Response.Description });
        }
        //В случае отмены списания товара, добавим метод отмены
        [HttpPost]
        public async Task<IActionResult> DeleteTransfer(int Id)
        {
            var Response = await _transferService.DeleteTransferService(Id);
            if (Response.StatusCode == Domain.Enum.StatusCode.Ok)
            {
                return Ok(new { description = Response.Description });
            }
            return BadRequest(new { description = Response.Description });
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
