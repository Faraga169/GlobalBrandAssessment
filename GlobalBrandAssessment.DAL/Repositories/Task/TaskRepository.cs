using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskModel = GlobalBrandAssessment.DAL.Data.Models.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.GlobalBrandDbContext;
using Microsoft.EntityFrameworkCore;


namespace GlobalBrandAssessment.DAL.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly GlobalbrandDbContext globalbrandDbContext;

        public TaskRepository(GlobalbrandDbContext globalbrandDbContext)
        {
            this.globalbrandDbContext = globalbrandDbContext;
        }
        public int Add(TaskModel task)
        {
            globalbrandDbContext.Tasks.Add(task);
            var result=globalbrandDbContext.SaveChanges();
            return result;

        }
        public List<TaskModel> Search(string searchname,int? managerid)
        {
            var query = globalbrandDbContext.Tasks.Include(e => e.AssignedEmployee).Include(e=>e.Attachments).Include(e=>e.Comments).Where(t=>t.AssignedEmployee.ManagerId == managerid).AsQueryable();
            if (!string.IsNullOrEmpty(searchname))
            {
                query = query.Where(e => e.Title.ToLower().Contains(searchname.ToLower()));
            }

            return query.ToList();
        }
        public int Delete(int? id)
        {
            var task = globalbrandDbContext.Tasks.Find(id);
            if (task == null)
            {
                return 0 ;
            }
            globalbrandDbContext.Tasks.Remove(task);
            var result = globalbrandDbContext.SaveChanges();
            return result;
        }

        public List<TaskModel> GetAllTasks(int? managerid)
        {
            var tasks = globalbrandDbContext.Tasks.Include(T=>T.AssignedEmployee).Include(T=>T.Attachments).Include(T=>T.Comments).Where(T=>T.AssignedEmployee.ManagerId==managerid).ToList();
            return tasks;
        }

       
        public TaskModel GetTaskById(int id)
        {
            var task = globalbrandDbContext.Tasks.Include(T=>T.Attachments).Include(T=>T.Comments).FirstOrDefault(T=>T.Id==id);
            return task;
        }

        public int Update(TaskModel task)
        {
            globalbrandDbContext.Tasks.Update(task);
            var result = globalbrandDbContext.SaveChanges();
            return result;

        }

        public List<TaskModel> GetTaskbyEmployeeId(int? id)
        {
            return globalbrandDbContext.Tasks.Include(T=>T.AssignedEmployee).Include(T=>T.AssignedEmployee.Manager).Include(T=>T.Attachments).Include(T=>T.Comments).Where(T=>T.EmployeeId==id).ToList();
        }

       

    }
}
