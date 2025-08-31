using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GlobalBrandAssessment.BL.DTOS.TaskDTO;
using GlobalBrandAssessment.BL.Services.Generic;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Repositories.Generic;
namespace GlobalBrandAssessment.BL.Services.Task
{
    public class TaskService : GenericService<Tasks>, ITaskService
    {
        private readonly ITaskRepository taskrepository;
        private readonly IMapper mapper;

        public TaskService(ITaskRepository taskrepository,IMapper mapper) : base(taskrepository)
        {
            this.taskrepository = taskrepository;
            this.mapper = mapper;
        }

        public Task<int> AddAsync(AddandUpdateTaskDTO entity)
        {
            return taskrepository.AddAsync(mapper.Map<AddandUpdateTaskDTO, Tasks>(entity));
        }

        public Task<int> DeleteAsync(int? id)
        {
            var result =taskrepository.DeleteAsync(id);
            return result;
        }

        public async Task<List<GetAllandSearchTaskDTO>> GetAllTasksAsync(int? managerid)
        {
            var tasks =await taskrepository.GetAllTasksAsync(managerid);
            var result=mapper.Map<List<Tasks>, List<GetAllandSearchTaskDTO>>(tasks);
            return result;
        }

        public async Task<List<Tasks>> GetTaskbyEmployeeIdAsync(int? id)
        {

            return await taskrepository.GetTaskbyEmployeeIdAsync(id);
        }

        public async Task<AddandUpdateTaskDTO> GetTaskByIdAsync(int id)
        {
            var task = await taskrepository.GetTaskByIdAsync(id);
            var result = mapper.Map<Tasks,AddandUpdateTaskDTO>(task);
            return result;
        }

        public async Task<List<GetAllandSearchTaskDTO>> SearchAsync(string searchname,int? managerid)
        {
            var task = await taskrepository.SearchAsync(searchname, managerid);
            var result = mapper.Map<List<Tasks>, List<GetAllandSearchTaskDTO>>(task);
            return result;
        }

        public Task< int> UpdateAsync(AddandUpdateTaskDTO entity)
        {
            return taskrepository.UpdateAsync(mapper.Map<AddandUpdateTaskDTO, Tasks>(entity));
        }
    }
}
