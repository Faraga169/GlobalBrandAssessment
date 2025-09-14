using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Repositories.Generic;
using GlobalBrandAssessment.GlobalBrandDbContext;
using Microsoft.EntityFrameworkCore;

namespace GlobalBrandAssessment.DAL.Repositories
{
    public class ManagerRepository :GenericRepository<Employee> ,IManagerRepository
    {
        private readonly GlobalbrandDbContext globalbrandDbContext;

        public ManagerRepository(GlobalbrandDbContext globalbrandDbContext):base(globalbrandDbContext)  //Injection
        {
            this.globalbrandDbContext = globalbrandDbContext;
        }


        public async Task<List<Employee>> SearchAsync(string searchname, int? managerid)
        {
            var query = globalbrandDbContext.Employees.Include(e => e.Department).Where(e => e.ManagerId == managerid).AsQueryable();

            if (!string.IsNullOrEmpty(searchname))
            {
                query = query.Where(e => e.FirstName.ToLower().Contains(searchname.ToLower())
                ||
                e.LastName.ToLower().Contains(searchname.ToLower())
                ||
                (e.Department!=null?e.Department.Name.ToLower():"No Department").Contains(searchname.ToLower()));

            }

            return await query.ToListAsync();
        }

        public async Task<Employee?> GetManagerByDepartmentIdAsync(int? deptId)
        {
           
            return await (from emp in globalbrandDbContext.Employees
                          join dept in globalbrandDbContext.Departments
                          on emp.Id equals dept.ManagerId
                          where dept.Id == deptId
                          select emp).FirstOrDefaultAsync();
        
    }

        public async Task<List<Employee>> GetAllManagersAsync()
        {
            return await globalbrandDbContext.Employees.Where(m => globalbrandDbContext.Employees.Any( e=>e.ManagerId == m.Id)).Select( m=> new Employee{ Id = m.Id,FirstName = $"{m.FirstName} {m.LastName}"}).ToListAsync();
        }
    }
}
