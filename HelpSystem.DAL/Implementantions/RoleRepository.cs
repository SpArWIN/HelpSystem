using HelpSystem.DAL.Interfasces;
using HelpSystem.Domain.Entity;

namespace HelpSystem.DAL.Implementantions
{
    public class RoleRepository : IBaseRepository<Role>
    {
        private readonly AppDbContext _dbContext;
        public RoleRepository(AppDbContext appDbContext)
        {
            _dbContext = appDbContext;
        }

        public async Task Create(Role entity)
        {
            await _dbContext.Roles.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public IQueryable<Role> GetAll()
        {
            return _dbContext.Roles;
        }

        public async Task<Role> Update(Role entity)
        {
            _dbContext.Roles.Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task Delete(Role entity)
        {
            _dbContext.Roles.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
