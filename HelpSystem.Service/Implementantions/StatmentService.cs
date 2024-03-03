using HelpSystem.DAL.Interfasces;
using HelpSystem.Domain.Entity;
using HelpSystem.Domain.Enum;
using HelpSystem.Domain.Extension;
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

        public StatmentService(IBaseRepository<User> account, IBaseRepository<Statement> service)
        {
            _accountService = account;
            _statmentService = service;
        }

        public async Task<BaseResponse<Statement>> CreateStatment(StatmentViewModel model, Guid id)
        {
            try
            {


                var findUser = _accountService.GetAll().FirstOrDefault(x => x.Id == id);

                //Конвертирую дату в нужный формат)
                //string dateString = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
                //DateTime parsedDate = DateTime.ParseExact(dateString, "dd.MM.yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                // Что то с ответам ajax не то и оформить красиво таблицу, добавив сортировку.

                var Service = new Statement
                {
                    DataCreated = DateTime.Now,
                    Reason = model.Reason,
                    Comments = model.Description,
                    User = findUser,
                    Status = StatusStatement.UnderConsideration,

                };

                await _statmentService.Create(Service);

                return new BaseResponse<Statement>()
                {
                    Data = Service,
                    Description = "Заявка успешно сформирована.Детали смотрите в таблице.",
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

        //Метод на завершение заявки, то есть ее полное закрытие.
        public async Task<BaseResponse<Statement>> UpdateStatment(AnswerStatmentViewModel asnw)
        {
            try
            {
                var ResUp = await _statmentService.GetAll()
                    .FirstOrDefaultAsync(x => x.ID == asnw.Id);
                /*
                 * Если завершенные заявки так и будут висеть, то нужно прописать логику..
                 * чтобы не обновлялись и отправки повтонрой не было, но я планирую их кидать в архив.
                 */
                if (asnw.Response == null || asnw.Response.Length < 10)
                {
                    return new BaseResponse<Statement>()
                    {
                        StatusCode = StatusCode.UnChanched,
                        Description = "Ответ является пустым или слишком коротким."
                    };
                }
                ResUp.ResponseAnswer = asnw.Response;
                ResUp.Status = StatusStatement.Completed;
                ResUp.DateCompleted = DateTime.Now;
                await _statmentService.Update(ResUp);
                return new BaseResponse<Statement>()
                {
                    StatusCode = StatusCode.Ok,
                    Description = "Заявка закрыта",
                    Data = ResUp

                };
            }
            catch (Exception e)
            {
                return new BaseResponse<Statement>()
                {
                    Description = $"{e.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        //Получение списка всех заявок в таблицу
        public async Task<DataTableResponse> GetAllStatments()
        {
            try
            {
                var QueryAll = await _statmentService.GetAll()
                    .Include(p => p.User)
                    .Include(p => p.User.Profile)
                    .Where(x => x.Status != StatusStatement.Completed)
                    .Select(x => new GetAllStatments
                    {
                        Id = x.ID,
                        DataCreated = x.DataCreated.ToString("dd.MM.yyyy HH:mm"),
                        Status = x.Status.GetDisplayName(),
                        Reason = x.Reason,
                        Description = x.Comments,
                        Name = x.User.Name,
                        LastName = x.User.Profile.LastName,
                        Patronymic = x.User.Profile.Surname,

                        FullName = $"{x.User.Profile.LastName} {x.User.Profile.Name} {x.User.Profile.Surname}"
                    })
                    .ToListAsync();
                var Count = QueryAll.Count();
                return new DataTableResponse()
                {
                    Data = QueryAll,
                    Total = Count
                };

            }
            catch (Exception e)
            {
                return new DataTableResponse()
                {
                    Data = null,
                    Total = 0
                };
            }
        }


        //Получение конкретной заявки
        public async Task<DataTableResponse> GetStatment(Guid id)
        {
            try
            {
                var Statment = await _statmentService.GetAll()
                    .Where(x => x.User.Id == id)
                    .Select(x => new StatmentViewModel
                    {
                        Id = x.ID,
                        DataCreated = x.DataCreated.ToString("dd.MM.yyyy HH:mm"),
                        Reason = x.Reason,
                        Description = x.Comments,
                        Status = x.Status.GetDisplayName(),
                    })
                    .ToListAsync();

                var Count = Statment.Count();
                return new DataTableResponse()
                {
                    Data = Statment,
                    Total = Count

                };
            }
            catch (Exception ex)
            {
                return new DataTableResponse()
                {
                    Data = null,
                    Total = 0
                };
            }
        }

        public async Task<BaseResponse<AnswerStatmentViewModel>> GetStat(Guid id)
        {
            try
            {
                var Response = await _statmentService.GetAll()
                    .Include(p => p.User.Profile)
                    .FirstOrDefaultAsync(x => x.ID == id);
                if (Response == null)
                {
                    return new BaseResponse<AnswerStatmentViewModel>()
                    {
                        StatusCode = StatusCode.NotFind,
                        Description = "Внутренняя ошибка, не найдено"
                    };
                }

                var Statment = new AnswerStatmentViewModel()
                {
                    Id = Response.ID,
                    Description = Response.Comments,
                    Reason = Response.Reason,
                    FullName =
                        $" {Response.User.Profile.LastName}  {Response.User.Profile.Name} {Response.User.Profile.Surname} "
                };

                return new BaseResponse<AnswerStatmentViewModel>()
                {
                    Data = Statment,
                    StatusCode = StatusCode.Ok
                };
            }
            catch (Exception e)
            {
                return new BaseResponse<AnswerStatmentViewModel>()
                {
                    StatusCode = StatusCode.InternalServerError,
                    Description = $"{e.Message}"
                };
            }
        }

        public async Task<BaseResponse<Statement>> UpdateStatusStat(Guid id, int newStat)
        {
            try
            {
                var res = await _statmentService.GetAll()
                    .FirstOrDefaultAsync(x => x.ID == id);
                if (res.Status == (StatusStatement)newStat)
                {
                    return new BaseResponse<Statement>()
                    {
                        StatusCode = StatusCode.UnChanched,

                    };
                }

                res.Status = (StatusStatement)newStat;

                await _statmentService.Update(res);
                return new BaseResponse<Statement>()
                {
                    Data = res,
                    StatusCode = StatusCode.Ok
                };

            }
            catch (Exception e)
            {
                return new BaseResponse<Statement>()
                {
                    StatusCode = StatusCode.InternalServerError,
                    Data = null,
                    Description = $"{e.Message}"
                };
            }
        }

        //Метод для отображения результат заявки от пользователя
        public async Task<BaseResponse<StatmentResultViewModel>> ShowAnswerStatment(Guid id)
        {
            try
            {
                var Answer = await _statmentService.GetAll()
                    .FirstOrDefaultAsync(x => x.ID == id);
                var ShowAnswer = new StatmentResultViewModel()
                {
                    Reason = Answer.Reason,
                    DateCreated = Answer.DataCreated.ToString("dd.MM.yyyy HH:mm"),
                    DateCompleted = Answer.DateCompleted?.ToString("dd.MM.yyyy HH:mm"),
                    Answer = Answer.ResponseAnswer,

                };
                return new BaseResponse<StatmentResultViewModel>()
                {
                    Data = ShowAnswer,
                    StatusCode = StatusCode.Ok,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<StatmentResultViewModel>()
                {
                    Description = $"{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

    }
}
