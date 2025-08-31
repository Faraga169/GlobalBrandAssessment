using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Repositories.Generic;
using GlobalBrandAssessment.GlobalBrandDbContext;
using Microsoft.EntityFrameworkCore;

namespace GlobalBrandAssessment.DAL.Repositories
{
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        private readonly GlobalbrandDbContext globalbrandDbContext;

        public CommentRepository(GlobalbrandDbContext globalbrandDbContext): base(globalbrandDbContext)
        {
            this.globalbrandDbContext = globalbrandDbContext;
        }

       

        public async Task<Comment?> GetByTaskId(int taskId)
        {
            return await globalbrandDbContext.Comments.AsNoTracking().FirstOrDefaultAsync(c => c.TaskId == taskId);
        }
    }
    }

