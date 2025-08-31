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

      

        public async Task<int> DeleteAsync(int? id)
        {
            var department = globalbrandDbContext.Departments.Include(d => d.Employees).FirstOrDefault(d => d.Id == id);

            if (department == null)
            {
                return 0; 
            }
            if (department.EmployeeCount > 0) {
                return -1;
            }
               
            globalbrandDbContext.Departments.Remove(department);
            return await globalbrandDbContext.SaveChangesAsync();
        }


        public async Task<List<Department>> SearchAsync(string searchname)
        {
            var query = globalbrandDbContext.Departments.Include(e=>e.Manager).Include(e=>e.Employees).AsQueryable();
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

        public async Task<Department> GetDepartmentById(int? id)
        {

             return await globalbrandDbContext.Departments.Include(d => d.Manager).FirstOrDefaultAsync(d => d.Id == id);
        }
    }
}

