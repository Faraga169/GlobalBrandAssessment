using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GlobalBrandAssessment.BL.DTOS.ManagerDTO;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Repositories;

namespace GlobalBrandAssessment.BL.Services
{
    public class EmployeeService:IEmployeeService
    {
        private readonly IEmployeeRepository employeeRepository;
        private readonly IMapper mapper;

        public EmployeeService(IEmployeeRepository employeeRepository,IMapper mapper)
        {
            this.employeeRepository = employeeRepository;
            this.mapper = mapper;
        }
        public Employee GetEmployeeById(int? employeeId)
        {
            var employee = employeeRepository.GetEmployeeById(employeeId);
            
            return employee;
        }

        public List<GetAllAndSearchManagerDTO> GetEmployeesByManager(int? ManagerId)
        {
            var employeelist = employeeRepository.GetEmployeesByManager(ManagerId);
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
    }
}
