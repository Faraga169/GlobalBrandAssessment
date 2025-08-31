using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;

namespace GlobalBrandAssessment.DAL.Repositories.Generic
{
    public interface IGenericRepository<T>
    {

        public Task<int> AddAsync(T entity);

        public Task<int> UpdateAsync(T entity);

    }
}
