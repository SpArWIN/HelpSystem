using HelpSystem.Domain.Entity;
using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Statment;

namespace HelpSystem.Service.Interfaces
{
    public interface IStatmentIService
    {
        Task<BaseResponse<Statement>> CreateStatment(StatmentViewModel model, Guid id);
        Task<BaseResponse<Statement>> UpdateStatment(Guid id);
        Task<DataTableResponse> GetAllStatments();
        Task<DataTableResponse> GetStatment(Guid id);


    }
}
