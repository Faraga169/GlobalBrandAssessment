using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace GlobalBrandAssessment.DAL.Data.Models
{
    public interface IManagerRepository:IGenericRepository<Employee>
    {


        public Task<int> DeleteAsync(int? id);

        public Task<List<Employee>> SearchAsync(string searchname,int?managerid);


        public Task<Employee> GetManagerByDepartmentIdAsync(int? deptId);

        public Task<List<Employee>> GetAllManagersAsync();

    }
}
