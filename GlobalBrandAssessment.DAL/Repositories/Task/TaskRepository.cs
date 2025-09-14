using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskModel = GlobalBrandAssessment.DAL.Data.Models.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.GlobalBrandDbContext;
using Microsoft.EntityFrameworkCore;
using GlobalBrandAssessment.DAL.Repositories.Generic;


namespace GlobalBrandAssessment.DAL.Repositories
{
    public class TaskRepository :GenericRepository<Tasks>, ITaskRepository
    {
        private readonly GlobalbrandDbContext globalbrandDbContext;

        public TaskRepository(GlobalbrandDbContext globalbrandDbContext):base(globalbrandDbContext)
        {
            this.globalbrandDbContext = globalbrandDbContext;
        }

        public async Task<List<TaskModel>> SearchAsync(string searchname, int? managerid)
        {
            var query = globalbrandDbContext.Tasks.Include(e => e.AssignedEmployee).Include(e => e.Attachments).Include(e => e.Comments).Where(t => t.AssignedEmployee.ManagerId == managerid).AsQueryable();
            if (!string.IsNullOrEmpty(searchname))
            {
                query = query.Where(e => e.Title.ToLower().Contains(searchname.ToLower()));
            }

            return await query.ToListAsync();
        }
      
           

        public async Task<List<TaskModel>> GetAllTasksAsync(int? managerid)
        {
            var tasks = globalbrandDbContext.Tasks.Include(T=>T.AssignedEmployee).Include(T=>T.Attachments).Include(T=>T.Comments).Where(T=>T.AssignedEmployee.ManagerId==managerid).ToListAsync();
            return await tasks;
        }

       
        public async Task<TaskModel?> GetTaskByIdAsync(int? id)
        {
            var task =await globalbrandDbContext.Tasks.Include(T=>T.Attachments).Include(T=>T.Comments).FirstOrDefaultAsync(T=>T.Id==id);
            return task;
        }

      

        public async Task<List<TaskModel>> GetTaskbyEmployeeIdAsync(int? id)
        {
            return await globalbrandDbContext.Tasks.Include(T=>T.AssignedEmployee).Include(T=>T.AssignedEmployee.Manager).Include(T=>T.Attachments).Include(T=>T.Comments).Where(T=>T.EmployeeId==id).ToListAsync();
        }

       

    }
}
