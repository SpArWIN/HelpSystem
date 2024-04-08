using HelpSystem.Domain.Entity;

namespace HelpSystem.DAL.Interfasces
{
    public interface IBaseRepository<T>
    {
        Task Create(T entity);
        IQueryable<T> GetAll();
        Task<T> Update(T entity);

        Task Delete(T entity);
    }
    //Реализую ещё один интерфейс, наследуясь от основного, для реализации одной функции для таблицы товаров
   
}
