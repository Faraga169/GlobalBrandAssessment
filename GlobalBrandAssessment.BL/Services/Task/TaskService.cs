using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GlobalBrandAssessment.BL.DTOS.TaskDTO;
using GlobalBrandAssessment.DAL.Data.Models;
namespace GlobalBrandAssessment.BL.Services.Task
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository taskRepository;
        private readonly IMapper mapper;

        public TaskService(ITaskRepository taskRepository,IMapper mapper)
        {
            this.taskRepository = taskRepository;
            this.mapper = mapper;
        }

        public int Add(AddandUpdateTaskDTO task)
        {
            
            var existtask=mapper.Map<AddandUpdateTaskDTO, Tasks>(task);
            var result = taskRepository.Add(existtask);
            return result;

        }

        public int Delete(int? id)
        {
            var result = taskRepository.Delete(id);
            return result;
        }

        public List<GetAllandSearchTaskDTO> GetAllTasks(int? managerid)
        {
            var tasks = taskRepository.GetAllTasks(managerid);
            var result=mapper.Map<List<Tasks>, List<GetAllandSearchTaskDTO>>(tasks);
            return result;
        }

        public List<Tasks> GetTaskbyEmployeeId(int? id)
        {

            return taskRepository.GetTaskbyEmployeeId(id);
        }

        public AddandUpdateTaskDTO GetTaskById(int id)
        {
            var task = taskRepository.GetTaskById(id);
            var result = mapper.Map<Tasks,AddandUpdateTaskDTO>(task);
            return result;
        }

        public List<GetAllandSearchTaskDTO> Search(string searchname,int? managerid)
        {
            var task = taskRepository.Search(searchname, managerid);
            var result = mapper.Map<List<Tasks>, List<GetAllandSearchTaskDTO>>(task);
            return result;
        }

        public int Update(AddandUpdateTaskDTO task)
        {
            var mapping = mapper.Map<AddandUpdateTaskDTO, Tasks>(task);
            var result = taskRepository.Update(mapping);
            return result;
        }
    }
}
