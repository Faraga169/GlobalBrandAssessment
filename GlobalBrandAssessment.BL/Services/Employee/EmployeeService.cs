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

        public async Task<PagedResult<GetAllAndSearchManagerDTO>> GetAllPagedAsync( int pageno = 1, int pagesize = 5, string sortcolumn = "FirstName")
        {
           
            var (employees, totalCount) = await unitOfWork.employeeRepository.GetAllPagedAsync( pageno, pagesize, sortcolumn);


            var result = mapper.Map<List<Employee>, List<GetAllAndSearchManagerDTO>>(employees);
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

        public async Task<List<GetAllAndSearchManagerDTO>> GetAll() { 
        var employee=await unitOfWork.employeeRepository.GetAll();
        var result = mapper.Map<List<Employee>, List<GetAllAndSearchManagerDTO>>(employee);
            return result;
        }

        public async Task<List<GetAllAndSearchManagerDTO>> GetEmployeesByManagerId(int? managerid) {
            var employee = await unitOfWork.employeeRepository.GetEmployeesByManagerId(managerid);
            var result = mapper.Map<List<Employee>, List<GetAllAndSearchManagerDTO>>(employee);
            return result;
        }
        public async Task<PagedResult<GetAllAndSearchManagerDTO>> GetEmployeesByManagerPagedAsync(int? ManagerId,int pageno = 1, int pagesize = 5,string sortcolumn="FirstName")
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

        public async Task<List<GetAllAndSearchManagerDTO>> GetEmployeesByDeptId(int id)
        {
            var employees = await unitOfWork.employeeRepository.GetEmployeesByDeptId(id);
            var result = mapper.Map<List<Employee>, List<GetAllAndSearchManagerDTO>>(employees);
            return result;
        }
    }
}
