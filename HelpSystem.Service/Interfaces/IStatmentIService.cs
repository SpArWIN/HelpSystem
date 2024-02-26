using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelpSystem.Domain.Entity;
using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Statment;

namespace HelpSystem.Service.Interfaces
{
    public interface IStatmentIService
    {
        Task<BaseResponse<Statement>> CreateStatment(StatmentViewModel model,Guid id);
    }
}
