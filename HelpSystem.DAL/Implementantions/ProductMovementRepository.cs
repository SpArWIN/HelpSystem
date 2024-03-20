using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelpSystem.DAL.Interfasces;
using HelpSystem.Domain.Entity;

namespace HelpSystem.DAL.Implementantions
{
    public class ProductMovementRepository : IBaseRepository<ProductMovement>
    {
        private readonly AppDbContext _appDbContext;
        public ProductMovementRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task Create(ProductMovement entity)
        {
            await _appDbContext.TransferProducts.AddAsync(entity);
            await _appDbContext.SaveChangesAsync();
        }

        public IQueryable<ProductMovement> GetAll()
        {
            return _appDbContext.TransferProducts;
        }

        public async Task<ProductMovement> Update(ProductMovement entity)
        {
            _appDbContext.TransferProducts.Update(entity);
            await _appDbContext.SaveChangesAsync();
            return entity;
        }

        public async Task Delete(ProductMovement entity)
        {
            _appDbContext.TransferProducts.Remove(entity);
            await _appDbContext.SaveChangesAsync();
            
        }
    }
}
