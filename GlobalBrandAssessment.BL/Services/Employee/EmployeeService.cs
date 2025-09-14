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

        public async Task<List<GetAllAndSearchManagerDTO>> GetEmployeesByManagerAsync(int? ManagerId)
        {
            var employeelist =await unitOfWork.employeeRepository.GetEmployeesByManager(ManagerId);

            if (employeelist == null || employeelist.Count == 0)
                return new List<GetAllAndSearchManagerDTO>();

            var result= mapper.Map<List<Employee>, List<GetAllAndSearchManagerDTO>>(employeelist);


            return result;
        }

        
        public async Task<string?> GetEmployeeImageUrlAsync(int id)
        {
            return await unitOfWork.employeeRepository.GetEmployeeImageUrlAsync(id)??null;
        }
    }
}
