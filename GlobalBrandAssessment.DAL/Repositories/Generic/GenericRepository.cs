using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.GlobalBrandDbContext;
using Microsoft.EntityFrameworkCore;

namespace GlobalBrandAssessment.DAL.Repositories.Generic
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly GlobalbrandDbContext globalbrandDbContext;

        public GenericRepository(GlobalbrandDbContext globalbrandDbContext)
        {
            this.globalbrandDbContext = globalbrandDbContext;
        }
        public async Task<int> AddAsync(T entity)
        {
            globalbrandDbContext.Set<T>().Add(entity);
            return await globalbrandDbContext.SaveChangesAsync();
        }



        
        public async Task<int> UpdateAsync(T entity)
        {
            globalbrandDbContext.Set<T>().Update(entity);
            return await globalbrandDbContext.SaveChangesAsync();
        }

        public async Task<List<T>> Search(Expression<Func<T, bool>> expression,int?value)
        {
            return (await globalbrandDbContext.Set<T>().Where(expression).ToListAsync());
        }
    }
}
