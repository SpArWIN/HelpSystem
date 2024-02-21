using HelpSystem.DAL.Interfasces;
using HelpSystem.Domain.Entity;

namespace HelpSystem.DAL.Implementantions
{
    public class AccountRepository : IBaseRepository<User>
    {
        private readonly AppDbContext _appDbContext;
        public AccountRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task Create(User entity)
        {
            await _appDbContext.Users.AddAsync(entity);
            await _appDbContext.SaveChangesAsync();
        }

        public IQueryable<User> GetAll()
        {
            return _appDbContext.Users;
        }

        public async Task<User> Update(User entity)
        {
            _appDbContext.Users.Update(entity);
            await _appDbContext.SaveChangesAsync();
            return entity;
        }

        public async Task Delete(User entity)
        {
            _appDbContext.Users.Remove(entity);
            await _appDbContext.SaveChangesAsync();
        }
    }
}
