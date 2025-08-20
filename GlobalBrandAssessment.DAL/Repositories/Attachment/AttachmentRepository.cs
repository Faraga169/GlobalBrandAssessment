using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.GlobalBrandDbContext;

namespace GlobalBrandAssessment.DAL.Repositories.Attachment
{
    public class AttachmentRepository : IAttachmentRepository
    {
        private readonly GlobalbrandDbContext globalbrandDbContext;

        public AttachmentRepository(GlobalbrandDbContext globalbrandDbContext)
        {
            this.globalbrandDbContext = globalbrandDbContext;
        }


        public int Add(Data.Models.Attachment attachment)
        {
            globalbrandDbContext.Attachments.Add(attachment);
            return globalbrandDbContext.SaveChanges();
        }
    }
}
