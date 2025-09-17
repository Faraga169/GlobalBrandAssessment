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
        public Task<Employee?> GetEmployeeById(int? employeeId);

        public Task<(List<Employee>, int TotalCount)> GetEmployeesByManagerPaged(int? ManagerId, int pageno = 0, int pagesize = 0, string sortcolumn = "FirstName");
        public  Task<string?> GetEmployeeImageUrlAsync(int id);
    }

}
