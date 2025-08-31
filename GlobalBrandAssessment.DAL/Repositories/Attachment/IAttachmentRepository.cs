using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Repositories.Generic;

namespace GlobalBrandAssessment.DAL.Repositories
{
    public interface IAttachmentRepository : IGenericRepository<DAL.Data.Models.Attachment>
    {


        public Task<DAL.Data.Models.Attachment?> GetByTaskId(int taskId);
    }
}
