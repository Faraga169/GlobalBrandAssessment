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
        public async Task<Employee> GetEmployeeById(int? employeeId)
        {
            return await globalbrandDbContext.Employees.Include(e => e.Department).Include(e => e.Manager).FirstOrDefaultAsync(e => e.Id == employeeId);
        }


        public async Task<List<Employee>> GetEmployeesByManager(int? ManagerId)
        {
            return await globalbrandDbContext.Employees.Include(e => e.Department).Include(e => e.Manager).Where(e => e.ManagerId == ManagerId).ToListAsync();
        }

        public async Task<string> GetEmployeeImageUrlAsync(int id)
        {
            return await globalbrandDbContext.Employees
                .Where(e => e.Id == id)
                .Select(e => e.ImageURL)
                .FirstOrDefaultAsync();
        }
    }
}
