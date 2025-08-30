using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.BL.DTOS.TaskDTO;
using GlobalBrandAssessment.DAL.Data.Models;

namespace GlobalBrandAssessment.BL.Services.Task
{
    public interface ITaskService
    {
        public List<GetAllandSearchTaskDTO> GetAllTasks(int? managerid);

        public AddandUpdateTaskDTO GetTaskById(int id);

        public int Add(AddandUpdateTaskDTO task);

        public int Update(AddandUpdateTaskDTO task);

        public int Delete(int? id);

        public List<GetAllandSearchTaskDTO> Search(string searchname, int? managerid);

        public List<Tasks> GetTaskbyEmployeeId(int? id);
    }
}
