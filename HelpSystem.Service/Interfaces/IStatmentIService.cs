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
            /// Метод завершение заявки
            /// </summary>
            /// <param name="answ"></param>
            /// <returns></returns>
        Task<BaseResponse<Statement>> UpdateStatment(AnswerStatmentViewModel answ);
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
                /// <summary>
                /// Метод обновления статуса заявки после её рассмотрения
                /// </summary>
                /// <param name="id"></param>
                /// <param name="newStat"></param>
                /// <returns></returns>
            Task<BaseResponse<Statement>> UpdateStatusStat(Guid id, int newStat);
        /// <summary>
        /// Метод получения ответа заявки (пользователь увидит детали)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
            Task<BaseResponse<StatmentResultViewModel>> ShowAnswerStatment(Guid id);

    }
}
