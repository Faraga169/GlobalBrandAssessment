using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Repositories.Generic;

namespace GlobalBrandAssessment.DAL.Repositories
{
    public interface IEmployeeRepository: IGenericRepository<Employee>
    {
        public Task<Employee> GetEmployeeById(int? employeeId);

        public Task<List<Employee>> GetEmployeesByManager(int? ManagerId);
        public  Task<string> GetEmployeeImageUrlAsync(int id);
    }

}
