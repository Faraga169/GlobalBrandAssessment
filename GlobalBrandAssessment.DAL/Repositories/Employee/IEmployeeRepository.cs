using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;

namespace GlobalBrandAssessment.DAL.Repositories
{
    public interface IEmployeeRepository
    {
        public Employee GetEmployeeById(int? employeeId);

        public List<Employee> GetEmployeesByManager(int? ManagerId);
    }

}
