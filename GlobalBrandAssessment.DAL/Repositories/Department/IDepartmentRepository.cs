using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Repositories.Generic;

namespace GlobalBrandAssessment.DAL.Repositories
{
    public interface IDepartmentRepository:IGenericRepository<Department>
    {
        public Task<List<Department>> GetAllAsync();


        public Task<List<Department>> SearchAsync(string searchname);

        public Task<Department?> GetDepartmentById(int? id);


        
    }
}
