using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GlobalBrandAssessment.BL.DTOS.ManagerDTO;
using GlobalBrandAssessment.BL.Services.Generic;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Repositories;
using GlobalBrandAssessment.DAL.Repositories.Generic;
using GlobalBrandAssessment.DAL.UnitofWork;
using GlobalBrandAssessment.GlobalBrandDbContext;

namespace GlobalBrandAssessment.BL.Services
{
    public class EmployeeService:GenericService<Employee, AddAndUpdateManagerDTO>, IEmployeeService
    {
        private readonly IUnitofWork unitOfWork;
   
       
        private readonly IMapper mapper;

        public EmployeeService(IUnitofWork unitOfWork,IMapper mapper) : base(unitOfWork, mapper)
        {
            this.unitOfWork = unitOfWork;
           
            this.mapper = mapper;
        }

       

        public async Task<Employee?> GetEmployeeByIdAsync(int? employeeId)
        {
            var employee =await unitOfWork.employeeRepository.GetEmployeeById(employeeId);
            if (employee == null)
                return null;

            return employee;
        }

        public async Task<PagedResult<GetAllAndSearchManagerDTO>> GetEmployeesByManagerPagedAsync(int? ManagerId,int pageno = 0, int pagesize = 0,string sortcolumn="FirstName")
        {
            var (employees, totalCount) = await unitOfWork.employeeRepository.GetEmployeesByManagerPaged(ManagerId,pageno,pagesize,sortcolumn);

        

            var result= mapper.Map<List<Employee>, List<GetAllAndSearchManagerDTO>>(employees);
            //var pagedResult=mapper.Map<List<GetAllAndSearchManagerDTO>, PagedResult<GetAllAndSearchManagerDTO>>(result);
            
            var pagedResult = new PagedResult<GetAllAndSearchManagerDTO>()
            {
                Items = result,
                PageNumber = pageno,
                PageSize = pagesize,
                TotalCount = totalCount,
            };

            return pagedResult;
          
        }

        
        public async Task<string?> GetEmployeeImageUrlAsync(int id)
        {
            return await unitOfWork.employeeRepository.GetEmployeeImageUrlAsync(id)??null;
        }
    }
}
