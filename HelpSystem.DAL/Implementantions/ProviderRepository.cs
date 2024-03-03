using HelpSystem.DAL.Interfasces;
using HelpSystem.Domain.Entity;

namespace HelpSystem.DAL.Implementantions
{
    public class ProviderRepository : IBaseRepository<Provider>
    {
        private readonly AppDbContext _appDbContext;

        public ProviderRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task Create(Provider entity)
        {
            await _appDbContext.AddAsync(entity);
            await _appDbContext.SaveChangesAsync();
        }

        public IQueryable<Provider> GetAll()
        {
            return _appDbContext.Providers;
        }

        public async Task<Provider> Update(Provider entity)
        {
            _appDbContext.Providers.Update(entity);
            await _appDbContext.SaveChangesAsync();
            return entity;
        }

        public async Task Delete(Provider entity)
        {
            _appDbContext.Providers.Remove(entity);
            await _appDbContext.SaveChangesAsync();
        }
    }
}
