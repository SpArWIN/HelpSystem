using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelpSystem.DAL.Interfasces;
using HelpSystem.Domain.Entity;

namespace HelpSystem.DAL.Implementantions
{
    public class BuyerRepository :IBaseRepository<Buyer>
    {
        private readonly AppDbContext _appDbContext;
        public BuyerRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task Create(Buyer entity)
        {
            await _appDbContext.AddAsync(entity);
            await _appDbContext.SaveChangesAsync();
        }

        public IQueryable<Buyer> GetAll()
        {
            return _appDbContext.Buyers;
        }

        public async Task<Buyer> Update(Buyer entity)
        {
            _appDbContext.Buyers.Update(entity);
            await _appDbContext.SaveChangesAsync();
            return entity;
        }

        public async Task Delete(Buyer entity)
        {
            _appDbContext.Buyers.Remove(entity);
            await _appDbContext.SaveChangesAsync();
        }
    }
}
