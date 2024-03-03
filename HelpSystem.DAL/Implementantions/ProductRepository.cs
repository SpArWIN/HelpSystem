using HelpSystem.DAL.Interfasces;
using HelpSystem.Domain.Entity;

namespace HelpSystem.DAL.Implementantions
{
    public class ProductRepository : IBaseRepository<Products>
    {
        private readonly AppDbContext _appDbContext;
        public ProductRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task Create(Products entity)
        {
            await _appDbContext.Products.AddAsync(entity);
            await _appDbContext.SaveChangesAsync();
        }

        public IQueryable<Products> GetAll()
        {
            return _appDbContext.Products;
        }

        public async Task<Products> Update(Products entity)
        {
            _appDbContext.Products.Update(entity);
            await _appDbContext.SaveChangesAsync();
            return entity;
        }

        public async Task Delete(Products entity)
        {
            _appDbContext.Products.Remove(entity);
            await _appDbContext.SaveChangesAsync();
        }
    }
}
