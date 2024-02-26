using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using HelpSystem.DAL.Implementantions;
using HelpSystem.DAL.Interfasces;
using HelpSystem.Domain.Entity;
using HelpSystem.Domain.Enum;
using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Statment;
using HelpSystem.Service.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace HelpSystem.Service.Implementantions
{
    public class StatmentService : IStatmentIService
    {
        //private AccountRepository _accountService;
        //private IStatmentIService _statmentService;
        private IBaseRepository<User> _accountService;
        private IBaseRepository<Statement> _statmentService;
     
        public StatmentService(IBaseRepository<User> account,IBaseRepository<Statement> service)
        {
           _accountService  = account;
           _statmentService = service;
        }

        public async Task<BaseResponse<Statement>> CreateStatment(StatmentViewModel model, Guid id)
        {
            try
            {
              
              
             var findUser = _accountService.GetAll().FirstOrDefault(x => x.Id == id);

                //Конвертирую дату в нужный формат)
             string dateString = DateTime.Today.ToString("dd.MM.yyyy HH:mm");
             DateTime parsedDate = DateTime.Parse(dateString);
                var Service = new Statement
                {
                    DataCreated = parsedDate,
                    Reason = model.Reason,
                    Comments = model.Description,
                    User = findUser,
                    Status = StatusStatement.UnderConsideration,

                };
               await _statmentService.Create(Service);

                return new BaseResponse<Statement>()
                {
                    Data = Service,
                    Description = "Заявка успешно сформирована",
                    StatusCode = StatusCode.Ok
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<Statement>()
                {
                    Description = $"{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
    }
}
