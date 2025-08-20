using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.GlobalBrandDbContext;
using Microsoft.EntityFrameworkCore;

namespace GlobalBrandAssessment.DAL.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly GlobalbrandDbContext globalbrandDbContext;

        public DepartmentRepository(GlobalbrandDbContext globalbrandDbContext)
        {
            this.globalbrandDbContext = globalbrandDbContext;
        }

        public int Add(Department department)
        {
            globalbrandDbContext.Departments.Add(department);
            return globalbrandDbContext.SaveChanges();
        }

        public int Delete(int? id)
        {
            var department = globalbrandDbContext.Departments
        .Include(d => d.Employees) 
        .FirstOrDefault(d => d.Id == id);

            if (department == null)
            {
                return 0; 
            }

            globalbrandDbContext.Departments.Remove(department);
            return globalbrandDbContext.SaveChanges();
        }


        public List<Department> Search(string searchname)
        {
            var query = globalbrandDbContext.Departments.Include(e=>e.Manager).Include(e=>e.Employees).AsQueryable();
            if (!string.IsNullOrEmpty(searchname))
            {
                query = query.Where(e => e.Name.ToLower().Contains(searchname.ToLower()));
            }
            return query.ToList();
        }
        public int Update(Department department)
        {
            globalbrandDbContext.Departments.Update(department);
            return globalbrandDbContext.SaveChanges();
        }


        public List<Department> GetAll()
        {
            return globalbrandDbContext.Departments.Include(d=>d.Manager).Include(d=> d.Employees).ToList();

        }

        public Department GetDepartmentById(int? id)
        {

                
            return globalbrandDbContext.Departments
                .Include(d => d.Manager)
                .FirstOrDefault(d => d.Id == id);
        }
    }
}

