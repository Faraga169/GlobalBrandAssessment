using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using GlobalBrandAssessment.DAL.Repositories;
using GlobalBrandAssessment.DAL.Repositories.Generic;
using GlobalBrandAssessment.DAL.UnitofWork;
using GlobalBrandAssessment.GlobalBrandDbContext;

namespace GlobalBrandAssessment.BL.Services.Generic
{
    public class GenericService<T,Tdto>:IGenericService<T,Tdto> where T : class 
    {
        private readonly IUnitofWork unitofWork;
        private readonly IMapper mapper;

        public GenericService(IUnitofWork unitofWork,IMapper mapper)
        {
            this.unitofWork = unitofWork;
            this.mapper = mapper;
        }

    

        public async Task<int> AddAsync(Tdto dto)
        {
            var entity=mapper.Map<Tdto, T>(dto);
            await unitofWork.Repository<T>().AddAsync(entity);
            var result = await unitofWork.CompleteAsync();
            return result > 0 ? result : 0;
        }

   

        public async Task<int> UpdateAsync(Tdto dto)
        {
            var entity= mapper.Map<Tdto, T>(dto);
            await unitofWork.Repository<T>().UpdateAsync(entity);
            var result = await unitofWork.CompleteAsync();
            return result > 0 ? result : 0;
        }
    }
}
