using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalBrandAssessment.BL.Services.Generic
{
    public interface IGenericService<T> 
    {
        public Task<int> AddAsync(T entity);

        public Task<int> UpdateAsync(T entity);



    }
}
