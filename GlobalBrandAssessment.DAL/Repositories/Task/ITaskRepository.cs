using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Repositories.Generic;
namespace GlobalBrandAssessment.DAL.Data.Models

{
    public interface ITaskRepository:IGenericRepository<Tasks>
    {
        public Task<List<Tasks>> GetAllTasksbyManagerIdAsyn(int? managerid);

        public Task<Tasks?> GetTaskByIdAsync(int? id);

        public Task<List<Tasks>> GetAll();
      
        public Task<List<Tasks>> SearchAsync(string searchname, int? managerid);

        public Task<List<Tasks>> GetTaskbyEmployeeIdAsync(int? id);


    }
}
