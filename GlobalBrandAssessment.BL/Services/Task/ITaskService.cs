using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.BL.DTOS.TaskDTO;
using GlobalBrandAssessment.BL.Services.Generic;
using GlobalBrandAssessment.DAL.Data.Models;

namespace GlobalBrandAssessment.BL.Services.Task
{
    public interface ITaskService:IGenericService<Tasks,AddandUpdateTaskDTO>
    {
        public Task<List<GetAllandSearchTaskDTO>> GetAllTasksAsync(int? managerid);

        public Task<AddandUpdateTaskDTO> GetTaskByIdAsync(int id);


        public Task<int> DeleteAsync(int? id);

        public Task<List<GetAllandSearchTaskDTO>> SearchAsync(string searchname, int? managerid);

        public Task<List<Tasks>> GetTaskbyEmployeeIdAsync(int? id);
    }
}
