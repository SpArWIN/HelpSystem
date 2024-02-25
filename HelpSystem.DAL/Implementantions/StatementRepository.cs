using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelpSystem.DAL.Interfasces;
using HelpSystem.Domain.Entity;

namespace HelpSystem.DAL.Implementantions
{
    public class StatementRepository :IBaseRepository<Statement>
    {
        private readonly AppDbContext _appDbContext;

        public StatementRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task Create(Statement entity)
        {
           await _appDbContext.Statements.AddAsync(entity);
           await _appDbContext.SaveChangesAsync();
        }

        public IQueryable<Statement> GetAll()
        {
            return _appDbContext.Statements;
        }

        public async Task<Statement> Update(Statement entity)
        {
            _appDbContext.Statements.Update(entity);
            await _appDbContext.SaveChangesAsync();
            return entity;
        }

        public Task Delete(Statement entity)
        {
            
        }
    }
}
