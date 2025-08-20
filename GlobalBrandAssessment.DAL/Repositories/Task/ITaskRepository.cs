using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace GlobalBrandAssessment.DAL.Data.Models

{
    public interface ITaskRepository
    {
        public List<Tasks> GetAllTasks(int? managerid);

        public Tasks GetTaskById(int id);

        public int Add(Tasks task);

        public int Update(Tasks task);

        public int Delete(int? id);
        public List<Tasks> Search(string searchname,int? managerid);

        public List<Tasks> GetTaskbyEmployeeId(int? id);


    }
}
