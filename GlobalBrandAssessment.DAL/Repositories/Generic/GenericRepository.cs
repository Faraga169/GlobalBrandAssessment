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
       
        public async Task AddAsync(T entity)
        {
            globalbrandDbContext.Set<T>().Add(entity);
           await Task.CompletedTask;
        }


        
        public async Task UpdateAsync(T entity)
        {
            globalbrandDbContext.Set<T>().Update(entity);
            await Task.CompletedTask;
        }


        public async Task DeleteAsync(T entity)
        {
            globalbrandDbContext.Set<T>().Remove(entity);
            await Task.CompletedTask;
        }

        

        //public async Task<List<T>> SearchAsync(Expression<Func<T, bool>> expression, int? valueparams ,Expression<Func<T, object>>[] includes)
        //{
        //    IQueryable<T> query = globalbrandDbContext.Set<T>();


        //    foreach (var include in includes)
        //    {
        //        query = query.Include(include);
        //    }

        //    if (expression != null)
        //    {
        //        query = query.Where(expression);
        //    }

        //    return await query.ToListAsync();
        //}


    }
}
