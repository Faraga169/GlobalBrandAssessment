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

namespace GlobalBrandAssessment.BL.Services
{
    public class EmployeeService:GenericService<Employee>, IEmployeeService
    {
        private readonly IEmployeeRepository employeeRepository;
        private readonly IMapper mapper;

        public EmployeeService(IEmployeeRepository employeeRepository,IMapper mapper) : base(employeeRepository)
        {
            this.employeeRepository = employeeRepository;
            this.mapper = mapper;
        }

        public async Task<int> AddAsync(AddAndUpdateManagerDTO entity)
        {
            return await employeeRepository.AddAsync(mapper.Map<AddAndUpdateManagerDTO, Employee>(entity));
        }

        public async Task<Employee> GetEmployeeByIdAsync(int? employeeId)
        {
            var employee =await employeeRepository.GetEmployeeById(employeeId);
            
            return employee;
        }

        public async Task<List<GetAllAndSearchManagerDTO>> GetEmployeesByManagerAsync(int? ManagerId)
        {
            var employeelist =await employeeRepository.GetEmployeesByManager(ManagerId);
           var result= mapper.Map<List<Employee>, List<GetAllAndSearchManagerDTO>>(employeelist);
            //var result=employeelist.Select(e => new GetAllAndSearchManagerDTO
            //{
            //    FirstName = e.FirstName,
            //    LastName = e.LastName,
            //    Salary = e.Salary,
            //    ImageURL = e.ImageURL,
            //    Department = e.Department.Name

            //}).ToList();

            return result;
        }

        public async Task<int> UpdateAsync(AddAndUpdateManagerDTO entity)
        {
            return await employeeRepository.UpdateAsync(mapper.Map<AddAndUpdateManagerDTO, Employee>(entity));
        }
    }
}
