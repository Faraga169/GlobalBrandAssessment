using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Repositories.Generic;
using GlobalBrandAssessment.GlobalBrandDbContext;
using Microsoft.EntityFrameworkCore;

namespace GlobalBrandAssessment.DAL.Repositories
{
    public class EmployeeRepository :GenericRepository<Employee>, IEmployeeRepository
    {
        private readonly GlobalbrandDbContext globalbrandDbContext;

        public EmployeeRepository(GlobalbrandDbContext globalbrandDbContext): base(globalbrandDbContext)
        {
            this.globalbrandDbContext = globalbrandDbContext;
        }
        public async Task<Employee?> GetEmployeeById(int? employeeId)
        {
            return await globalbrandDbContext.Employees.Include(e => e.Department).Include(e => e.Manager).FirstOrDefaultAsync(e => e.Id == employeeId);
        }

        public async Task<(List<Employee>, int TotalCount)> GetAllPagedAsync(int pageno = 1, int pagesize = 5, string sortcolumn = "FirstName")
        {
            var query = globalbrandDbContext.Employees
                .Include(e => e.Department)
                .Include(e => e.Manager).OrderBy(e=>e.FirstName);

            var totalCount = query.Count();


            var employee = await query.Skip((pageno - 1) * pagesize).Take(pagesize).ToListAsync();

            return (employee, totalCount);
        }


        public async Task<(List<Employee>,int TotalCount)> GetEmployeesByManagerPaged(int? ManagerId,int pageno=1,int pagesize=5,string sortcolumn="FirstName")
        {
            var query=  globalbrandDbContext.Employees.Include(e => e.Department).Include(e => e.Manager).Where(e => e.ManagerId == ManagerId);
            var Totalcount = query.Count();
           
                query = sortcolumn switch
                {
                    "LastName" => query.OrderBy(e => e.LastName),
                    "Department" => query.OrderBy(e => e.Department.Name),
                    _ => query.OrderBy(e => e.FirstName)
                };
            
            var employee = await query.Skip((pageno - 1) * pagesize).Take(pagesize).ToListAsync();
            return (employee, Totalcount);
        }

        public async Task<List<Employee>> GetEmployeesByManagerId(int? managerid) { 
        
            var employees=await globalbrandDbContext.Employees.Where(e => e.ManagerId == managerid).ToListAsync();
            return employees;
        }

        public async Task<List<Employee>> GetAll()
        {

           
            var nonManagers = await globalbrandDbContext.Employees
                .Where(e => !globalbrandDbContext.Departments.Any(d => d.ManagerId == e.Id))
                .ToListAsync();

            return nonManagers;
        }
        public async Task<string?> GetEmployeeImageUrlAsync(int id)
        {
            return await globalbrandDbContext.Employees.Where(e => e.Id == id).Select(e => e.ImageURL).FirstOrDefaultAsync();
        }

        public async Task<List<Employee>> GetEmployeesByDeptId(int id) {
            
            return await globalbrandDbContext.Employees.Where(e => e.DeptId == id&& e.ManagerId!=null).ToListAsync();
        }


     
    }
}
