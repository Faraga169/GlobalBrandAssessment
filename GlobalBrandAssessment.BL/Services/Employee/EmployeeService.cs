using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Repositories;

namespace GlobalBrandAssessment.BL.Services
{
    public class EmployeeService:IEmployeeService
    {
        private readonly IEmployeeRepository employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            this.employeeRepository = employeeRepository;
        }
        public Employee GetEmployeeById(int? employeeId)
        {
            return employeeRepository.GetEmployeeById(employeeId);
        }

        public List<Employee> GetEmployeesByManager(int? ManagerId)
        {
            return employeeRepository.GetEmployeesByManager(ManagerId);
        }
    }
}
