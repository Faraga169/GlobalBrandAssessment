using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;

namespace GlobalBrandAssessment.BL.Services
{
    public interface IEmployeeService
    {
        public Employee GetEmployeeById(int? employeeId);

        public List<Employee> GetEmployeesByManager(int? ManagerId);
    }
}
