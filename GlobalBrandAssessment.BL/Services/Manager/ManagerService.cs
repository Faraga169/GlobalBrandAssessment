using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Repositories;
using GlobalBrandAssessment.GlobalBrandDbContext;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace GlobalBrandAssessment.BL.Services.Manager
{
    public class ManagerService : IManagerService
    {
        
        private readonly IManagerRepository managerRepository;

        public ManagerService(IManagerRepository managerRepository)
        {
            
            this.managerRepository = managerRepository;
        }
        public int Add(Employee employee)
        {
            return managerRepository.Add(employee);
        }

        public int Delete(int? id)
        {
            return managerRepository.Delete(id);
        }

       

        public List<Employee> Search(string searchname, int? managerid)
        {

            return managerRepository.Search(searchname,managerid);
        }

        public int Update(Employee employee)
        {
            return managerRepository.Update(employee);
        }

       

        public Employee GetManagerByDepartmentId(int? deptId)
        {
            return managerRepository.GetManagerByDepartmentId(deptId) ?? throw new ArgumentException("Manager not found for the specified department ID.", nameof(deptId));
        }

        public List<Employee> GetAllManagers()
        {
            return managerRepository.GetAllManagers();
        }
    }
}