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

namespace GlobalBrandAssessment.DAL.Repositories.Attachment
{
    public class AttachmentRepository:GenericRepository<DAL.Data.Models.Attachment>, IAttachmentRepository
    {
        private readonly GlobalbrandDbContext globalbrandDbContext;

        public AttachmentRepository(GlobalbrandDbContext globalbrandDbContext): base(globalbrandDbContext)
        {
            this.globalbrandDbContext = globalbrandDbContext;
        }

       
        public async Task<DAL.Data.Models.Attachment?> GetByTaskId(int taskId)
        {
            return await globalbrandDbContext.Attachments.AsNoTracking().FirstOrDefaultAsync(c => c.TaskId == taskId);
        }
    }
}