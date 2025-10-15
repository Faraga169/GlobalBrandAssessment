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
            await unitofWork.Repository<IManagerRepository, Employee>().AddAsync(entity);
            await unitofWork.CompleteAsync();
            return entity.Id;
        }

        public async Task<int> DeleteAsync(int? id)
        {
            var manager = await unitofWork.Repository<IEmployeeRepository, Employee>().GetEmployeeById(id);
            if (manager == null)
                return 0;

            
            var employees = await unitofWork.Repository<IEmployeeRepository, Employee>().GetAll();
            bool hasEmployees = employees.Any(e => e.ManagerId == id);

            if (hasEmployees)
            {
                
                return -1;
            }

            var departments = await unitofWork.Repository<IDepartmentRepository,Department>().GetAllAsync();
            bool hasDepartment = departments.Any(d => d.ManagerId == id);

            if (hasDepartment)
            {
                
                return -2;
            }

            await unitofWork.Repository<IManagerRepository,Employee>().DeleteAsync(manager);
            var result = await unitofWork.CompleteAsync();
            return result > 0 ? result : 0;
        }

        public async Task<int> DemoteManagerToEmployeeAsync(int? managerId) { 
        await unitofWork.Repository<IManagerRepository,Employee>().DemoteManagerToEmployeeAsync(managerId);
        var result= await unitofWork.CompleteAsync();
            return result > 0 ? result : 0;
        }

        public async Task<PagedResult<GetAllAndSearchManagerDTO>> SearchAsync(string searchname,int? managerid,int pageno,int pagesize,string sortColumn)
        {
            var (employee,TotalCount) = await unitofWork.Repository<IManagerRepository, Employee>().SearchAsync(searchname,managerid,pageno,pagesize);
            var SearchManagerDTO = mapper.Map<List<Employee>, List<GetAllAndSearchManagerDTO>>(employee);

            return new PagedResult<GetAllAndSearchManagerDTO>
            {
                Items = SearchManagerDTO,
                PageNumber = pageno,
                PageSize = pagesize,
                TotalCount = TotalCount
            };
            

        }

    
        




        public Task<Employee?> GetManagerByDepartmentIdAsync(int? deptId)
        {
            
            return unitofWork.Repository<IManagerRepository, Employee>().GetManagerByDepartmentIdAsync(deptId) ;
        }

        public async Task<List<Employee>> GetAllManagersAsync()
        {
            
            return await unitofWork.Repository<IManagerRepository, Employee>().GetAllManagersAsync();
        }

        
    }
}