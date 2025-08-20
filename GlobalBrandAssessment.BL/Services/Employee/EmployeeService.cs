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
            if (!employeeId.HasValue || employeeId <= 0)
                throw new ArgumentException("Employee ID cannot be null, negative, or zero.", nameof(employeeId));
            return employeeRepository.GetEmployeeById(employeeId)?? throw new ArgumentException("Employee not found in the system.", nameof(employeeId));
        }

        public List<Employee> GetEmployeesByManager(int? ManagerId)
        {
            return employeeRepository.GetEmployeesByManager(ManagerId);
        }
    }
}
