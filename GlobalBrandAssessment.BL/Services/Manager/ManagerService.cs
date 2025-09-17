using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using GlobalBrandAssessment.BL.DTOS.DepartmentDTO;
using GlobalBrandAssessment.BL.DTOS.ManagerDTO;
using GlobalBrandAssessment.BL.Services.Generic;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Repositories;
using GlobalBrandAssessment.DAL.Repositories.Generic;
using GlobalBrandAssessment.DAL.UnitofWork;
using GlobalBrandAssessment.GlobalBrandDbContext;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace GlobalBrandAssessment.BL.Services.Manager
{
    public class ManagerService : GenericService<Employee, AddAndUpdateManagerDTO>, IManagerService
    {
        private readonly IUnitofWork unitofWork;
        private readonly IMapper mapper;

        public ManagerService(IUnitofWork unitofWork,IMapper mapper):base(unitofWork, mapper)
        {
            this.unitofWork = unitofWork;
            this.mapper = mapper;
        }

        public async Task<int> Add(AddAndUpdateManagerDTO addAndUpdateManagerDTO)
        {
            var entity = mapper.Map<AddAndUpdateManagerDTO, Employee>(addAndUpdateManagerDTO);
            await unitofWork.ManagerRepository.AddAsync(entity);
            await unitofWork.CompleteAsync();
            return entity.Id;
        }

        public async Task<int> DeleteAsync(int? id)
        {
            var manager = await unitofWork.employeeRepository.GetEmployeeById(id);
            if (manager == null)
                return 0;
            await unitofWork.ManagerRepository.DeleteAsync(manager);
            var result=await unitofWork.CompleteAsync();
            return result > 0 ? result : 0;
        }



        public async Task<List<GetAllAndSearchManagerDTO>> SearchAsync(string searchname,int?managerid)
        {
            //.And .Or Extension Methods for Expression Func Predicate
            //Expression<Func<Employee, bool>> searchExpression = x => true;
            //if (string.IsNullOrEmpty(searchname)) {

            //    searchExpression = x => x.FirstName.Contains(searchname) || x.LastName.Contains(searchname);
            //}

            var employee = await unitofWork.ManagerRepository.SearchAsync(searchname,managerid);
            var SearchManagerDTO = mapper.Map<List<Employee>, List<GetAllAndSearchManagerDTO>>(employee);

           
            return SearchManagerDTO;
        }

    
        




        public Task<Employee?> GetManagerByDepartmentIdAsync(int? deptId)
        {
            
            return unitofWork.ManagerRepository.GetManagerByDepartmentIdAsync(deptId) ;
        }

        public async Task<List<Employee>> GetAllManagersAsync()
        {
            
            return await unitofWork.ManagerRepository.GetAllManagersAsync();
        }

        
    }
}