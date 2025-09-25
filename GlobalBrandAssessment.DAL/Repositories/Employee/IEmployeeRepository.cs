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

        public Task<(List<Employee>, int TotalCount)> GetEmployeesByManagerPaged(int? ManagerId, int pageno = 1, int pagesize = 5, string sortcolumn = "FirstName");
        public Task<(List<Employee>, int TotalCount)> GetAllPagedAsync( int pageno=1 , int pagesize=5, string sortcolumn="FirstName");
        public  Task<string?> GetEmployeeImageUrlAsync(int id);

        public  Task<List<Employee>> GetAll();

        public Task<List<Employee>> GetEmployeesByManagerId(int? managerid);
        


        public Task<List<Employee>> GetEmployeesByDeptId(int id);
    }

}
