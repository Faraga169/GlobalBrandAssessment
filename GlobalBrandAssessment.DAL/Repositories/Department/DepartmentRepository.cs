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
    public class DepartmentRepository :GenericRepository<Department>, IDepartmentRepository
    {
        private readonly GlobalbrandDbContext globalbrandDbContext;

        public DepartmentRepository(GlobalbrandDbContext globalbrandDbContext) : base(globalbrandDbContext)
        {
            this.globalbrandDbContext = globalbrandDbContext;
        }



        public async Task<List<Department>> SearchAsync(string searchname)
        {
            var query = globalbrandDbContext.Departments.Include(e => e.Manager).Include(e => e.Employees).AsQueryable();
            if (!string.IsNullOrEmpty(searchname))
            {
                query = query.Where(e => e.Name.ToLower().Contains(searchname.ToLower()));
            }
            return await query.ToListAsync();
        }



        public async Task<List<Department>> GetAllAsync()
        {
            return await globalbrandDbContext.Departments.Include(d=>d.Manager).Include(d=> d.Employees).ToListAsync();

        }

        public async Task<Department?> GetDepartmentById(int? id)
        {

             return await globalbrandDbContext.Departments.Include(d => d.Manager).Include(d=>d.Employees).AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<List<Department>> GetDepartmentsByManagerId(int? managerId)
        {
            return await globalbrandDbContext.Departments
                .Include(d => d.Manager)
                .Include(d => d.Employees)
                .Where(d => d.ManagerId == managerId)
                .ToListAsync();
        }
    }
}

