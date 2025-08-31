using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Repositories;
using GlobalBrandAssessment.DAL.Repositories.Generic;
using GlobalBrandAssessment.GlobalBrandDbContext;

namespace GlobalBrandAssessment.BL.Services.Generic
{
    public class GenericService<T>:IGenericService<T> where T : class
    {
        private readonly IGenericRepository<T> genericRepository;

        public GenericService(IGenericRepository<T> genericRepository)
        {
            this.genericRepository = genericRepository;
        }

    

        public async Task<int> AddAsync(T entity)
        {
            return await genericRepository.AddAsync(entity);
        }




        public async Task<int> UpdateAsync(T entity)
        {
            return await genericRepository.UpdateAsync(entity);
        }
    }
}
