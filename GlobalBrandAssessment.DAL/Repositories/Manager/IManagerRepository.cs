using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GlobalBrandAssessment.DAL.Data.Models
{
    public interface IManagerRepository
    {

        public int Add(Employee employee);

        public int Update(Employee employee);

        public int Delete(int? id);

        public List<Employee> Search(string searchname,int?managerid);


        public Employee GetManagerByDepartmentId(int? deptId);

        public List<Employee> GetAllManagers();

    }
}
