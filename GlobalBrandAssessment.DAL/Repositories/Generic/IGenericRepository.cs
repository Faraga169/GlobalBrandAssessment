using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;

namespace GlobalBrandAssessment.DAL.Repositories.Generic
{
    public interface IGenericRepository<T>
    {

        public Task AddAsync(T entity);

        public Task UpdateAsync(T entity);

        //public Task<List<T>> SearchAsync(Expression<Func<T, bool>> expression, int? valueparams, Expression<Func<T, object>>[] includes);

        public Task DeleteAsync(T entity);

       


    }
}
