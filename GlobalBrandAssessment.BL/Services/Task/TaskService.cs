using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using GlobalBrandAssessment.BL.DTOS.ManagerDTO;
using GlobalBrandAssessment.BL.DTOS.TaskDTO;
using GlobalBrandAssessment.BL.Services.Generic;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Repositories;
using GlobalBrandAssessment.DAL.Repositories.Generic;
using GlobalBrandAssessment.DAL.UnitofWork;
namespace GlobalBrandAssessment.BL.Services.Task
{
    public class TaskService : GenericService<Tasks,AddandUpdateTaskDTO>, ITaskService
    {
        private readonly IUnitofWork unitofWork;
        private readonly IMapper mapper;

        public TaskService(IUnitofWork unitofWork,IMapper mapper) : base(unitofWork, mapper)
        {
            
            this.unitofWork = unitofWork;
            this.mapper = mapper;
        }


        public async Task<int> DeleteAsync(int? id)
        {
            var task = await unitofWork.taskRepository.GetTaskByIdAsync(id);
            if (task is null)
                return 0;

            await unitofWork.taskRepository.DeleteAsync(task);

            var result = await unitofWork.CompleteAsync();
            return result>0?result:0;
        }

        
        public async Task<List<GetAllandSearchTaskDTO>> GetAllTasksbyManagerIdAsync(int? managerid)
        {
            var tasks =await unitofWork.taskRepository.GetAllTasksbyManagerIdAsyn(managerid);
            var result=mapper.Map<List<Tasks>, List<GetAllandSearchTaskDTO>>(tasks);
            return result;
        }


        public async Task<List<GetAllandSearchTaskDTO>> GetAll()
        {
            var tasks = await unitofWork.taskRepository.GetAll();
            var result = mapper.Map<List<Tasks>, List<GetAllandSearchTaskDTO>>(tasks);
            return result;
        }
        public async Task<List<Tasks>> GetTaskbyEmployeeIdAsync(int? id)
        {

            return await unitofWork.taskRepository.GetTaskbyEmployeeIdAsync(id);
        }

        public async Task<AddandUpdateTaskDTO> GetTaskByIdAsync(int id)
        {
            var task = await unitofWork.taskRepository.GetTaskByIdAsync(id);
            if (task == null)
                return null!;
            var result = mapper.Map<Tasks,AddandUpdateTaskDTO>(task);
            return result;
        }

        public async Task<List<GetAllandSearchTaskDTO>> SearchAsync(string searchname, int? managerid)
        {
            var task = await unitofWork.taskRepository.SearchAsync(searchname, managerid);
            var result = mapper.Map<List<Tasks>, List<GetAllandSearchTaskDTO>>(task);
            return result;
        }


    }
}
