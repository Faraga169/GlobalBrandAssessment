using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;
namespace GlobalBrandAssessment.BL.Services.Task
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            this.taskRepository = taskRepository;
        }

        public int Add(Tasks task)
        {
            var result = taskRepository.Add(task);
            return result;

        }

        public int Delete(int? id)
        {
            var result = taskRepository.Delete(id);
            return result;
        }

        public List<Tasks> GetAllTasks(int? managerid)
        {
            var tasks = taskRepository.GetAllTasks(managerid);
            return tasks;
        }

        public List<Tasks> GetTaskbyEmployeeId(int? id)
        {

            return taskRepository.GetTaskbyEmployeeId(id);
        }

        public Tasks GetTaskById(int id)
        {
            var task = taskRepository.GetTaskById(id);
            return task;
        }

        public List<Tasks> Search(string searchname,int? managerid)
        {
           
            return taskRepository.Search(searchname, managerid);
        }

        public int Update(Tasks task)
        {
            var result = taskRepository.Update(task);
            return result;
        }
    }
}
