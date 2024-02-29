using HelpSystem.Domain.Entity;
using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Statment;

namespace HelpSystem.Service.Interfaces
{
    public interface IStatmentIService
    {
        /// <summary>
        /// Метод создания заявки
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id"></param>
        /// <returns>Возвращает BaseResponse </returns>
        Task<BaseResponse<Statement>> CreateStatment(StatmentViewModel model, Guid id);
            /// <summary>
            /// Метод обновление заявки, скорее всего будет обновлён именно статус
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
        Task<BaseResponse<Statement>> UpdateStatment(Guid id);
            /// <summary>
            /// Метод получения списка всех заявок
            /// </summary>
            /// <returns></returns>
        Task<DataTableResponse> GetAllStatments();
            /// <summary>
            /// Метод получения заявок пользователя, это важно, передеча будет осуществляться в таблицу
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
        Task<DataTableResponse> GetStatment(Guid id);
            /// <summary>
            /// Тут получение одной заявки конкретного пользователя
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            
            Task<BaseResponse<AnswerStatmentViewModel>>GetStat(Guid id);

            Task<BaseResponse<Statement>> UpdateStatusStat(Guid id, int newStat);
    }
}
