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
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly GlobalbrandDbContext globalbrandDbContext;

        public EmployeeRepository(GlobalbrandDbContext globalbrandDbContext)
        {
            this.globalbrandDbContext = globalbrandDbContext;
        }
        public Employee GetEmployeeById(int? employeeId)
        {
            return globalbrandDbContext.Employees.Include(e => e.Department).Include(e => e.Manager).FirstOrDefault(e => e.Id == employeeId);
        }


        public List<Employee> GetEmployeesByManager(int? ManagerId)
        {
            return globalbrandDbContext.Employees.Include(e => e.Department).Include(e => e.Manager).Where(e => e.ManagerId == ManagerId).ToList();
        }
    }
}
