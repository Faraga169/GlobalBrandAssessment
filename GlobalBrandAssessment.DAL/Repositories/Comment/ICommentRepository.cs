using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Repositories.Generic;
using GlobalBrandAssessment.GlobalBrandDbContext;

namespace GlobalBrandAssessment.DAL.Repositories
{
    public interface ICommentRepository:IGenericRepository<Comment>
    {


        public Task<Comment?> GetByTaskId(int taskId);
       


    }
}
