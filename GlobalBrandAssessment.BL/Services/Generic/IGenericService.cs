using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.GlobalBrandDbContext;
using Microsoft.EntityFrameworkCore;

namespace GlobalBrandAssessment.BL.Services.Generic
{
    public interface IGenericService<T,Tdto> 
    {
        public Task<int> AddAsync(Tdto entity);

        public Task<int> UpdateAsync(Tdto entity);


    }
}
