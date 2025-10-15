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
using Microsoft.IdentityModel.Tokens;

namespace GlobalBrandAssessment.DAL.Repositories
{
    public class ManagerRepository :GenericRepository<Employee> ,IManagerRepository
    {
        private readonly GlobalbrandDbContext globalbrandDbContext;

        public ManagerRepository(GlobalbrandDbContext globalbrandDbContext):base(globalbrandDbContext)  //Injection
        {
            this.globalbrandDbContext = globalbrandDbContext;
        }

       
        public async Task<(List<Employee>,int TotalCount)> SearchAsync(string searchname, int? managerid,int pageno,int pagesize)
        {
            var query = globalbrandDbContext.Employees.Include(e => e.Department).AsQueryable();

            if(managerid.HasValue)
            {
                query = query.Where(e => e.ManagerId == managerid);
            }

            if (!string.IsNullOrEmpty(searchname))
            {
                query = query.Where(e => e.FirstName.ToLower().Contains(searchname.ToLower())
                ||
                e.LastName.ToLower().Contains(searchname.ToLower())
                ||
                (e.Department!=null?e.Department.Name.ToLower():"No Department").Contains(searchname.ToLower()));

            }

            var totalCount = await query.CountAsync();

            var employees = await query
                .OrderBy(e => e.FirstName)
                .Skip((pageno - 1) * pagesize)
                .Take(pagesize)
                .ToListAsync();

            return (employees, totalCount);

        }

        public async Task<Employee?> GetManagerByDepartmentIdAsync(int? deptId)
        {
           
            return await (from emp in globalbrandDbContext.Employees
                          join dept in globalbrandDbContext.Departments
                          on emp.Id equals dept.ManagerId
                          where dept.Id == deptId
                          select emp).AsNoTracking().FirstOrDefaultAsync();
        
    }

        public async Task DemoteManagerToEmployeeAsync(int? managerId)
        {

            var subordinates = await globalbrandDbContext.Employees
                .Where(e => e.ManagerId == managerId)
                .ToListAsync();
            if (subordinates.Count > 0)
            {
                foreach (var sub in subordinates)
                {
                    sub.ManagerId = null;
                }
            }
            else
            {


                var department = await globalbrandDbContext.Departments
                    .FirstOrDefaultAsync(d => d.ManagerId == managerId);

                if (department != null)
                {

                    var employeesWithoutManager = await globalbrandDbContext.Employees
                        .Where(e => e.ManagerId == null && e.DeptId == department.Id&&e.Id!=managerId)
                        .ToListAsync();



                    foreach (var emp in employeesWithoutManager)
                    {
                        emp.ManagerId = managerId;
                    }

                   
                }
            }
        }

        

        public async Task<List<Employee>> GetAllManagersAsync()
        {
            return await globalbrandDbContext.Employees.Where(m => globalbrandDbContext.Employees.Any( e=>e.ManagerId == m.Id)).Select( m=> new Employee{ Id = m.Id,FirstName = $"{m.FirstName} {m.LastName}"}).ToListAsync();
        }

      
    }
}
