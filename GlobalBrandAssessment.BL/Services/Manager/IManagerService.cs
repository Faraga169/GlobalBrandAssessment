using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Repositories;

namespace GlobalBrandAssessment.BL.Services.Manager
{
    public interface IManagerService
    {


        public int Add(Employee employee);

        public int Update(Employee employee);

        public int Delete(int? id);

        public List<Employee> Search(string searchname, int? managerid);



        public List<Employee> GetAllManagers();

        public Employee GetManagerByDepartmentId(int? deptId);
       
    }
}
