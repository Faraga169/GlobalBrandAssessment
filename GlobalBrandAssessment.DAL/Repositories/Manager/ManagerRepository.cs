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
    public class ManagerRepository : IManagerRepository
    {
        private readonly GlobalbrandDbContext globalbrandDbContext;

        public ManagerRepository(GlobalbrandDbContext globalbrandDbContext)
        {
            this.globalbrandDbContext = globalbrandDbContext;
        }

        public int Add(Employee employee)
        {
            globalbrandDbContext.Employees.Add(employee);
            return globalbrandDbContext.SaveChanges();
        }

        public int Delete(int? id)
        {
            var employee = globalbrandDbContext.Employees.Find(id);
            if (employee == null)
            {
                return 0;
            }
            globalbrandDbContext.Employees.Remove(employee);
            return globalbrandDbContext.SaveChanges();
        }

        public List<Employee> Search(string searchname, int? managerid)
        {
            var query = globalbrandDbContext.Employees.Include(e=>e.Department).Where(e=>e.ManagerId==managerid).AsQueryable();
            if (!string.IsNullOrEmpty(searchname))
            {
                query = query.Where(e => e.FirstName.ToLower().Contains(searchname.ToLower())
                ||
                e.LastName.ToLower().Contains(searchname.ToLower())
                || 
                e.Department.Name.ToLower().Contains(searchname.ToLower()));
               
            }

            return query.ToList();
        }


        public int Update(Employee employee)
        {
            globalbrandDbContext.Employees.Update(employee);
            return globalbrandDbContext.SaveChanges();
        }

       

        

        public Employee GetManagerByDepartmentId(int? deptId)
        {
           
            return (from emp in globalbrandDbContext.Employees
                    join dept in globalbrandDbContext.Departments
                        on emp.Id equals dept.ManagerId
                    where dept.Id == deptId
                    select emp)
                   .FirstOrDefault();
        
    }

        public List<Employee> GetAllManagers()
        {
            return globalbrandDbContext.Employees.Where(m => globalbrandDbContext.Employees.Any(e => e.ManagerId == m.Id)).ToList();
        }
    }
}
