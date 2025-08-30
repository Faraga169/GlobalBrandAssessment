using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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


        public int AddOrUpdate(DAL.Data.Models.Attachment attachment)
        {
            var existingAttachment = globalbrandDbContext.Attachments
                .FirstOrDefault(a => a.TaskId == attachment.TaskId);

            if (existingAttachment != null)
            {

                existingAttachment.File = attachment.File;
                existingAttachment.FilePath = attachment.FilePath;
                existingAttachment.UploadedById = attachment.UploadedById;
                globalbrandDbContext.Attachments.Update(existingAttachment);
            }
            else
            {

                globalbrandDbContext.Attachments.Add(attachment);
            }

            return globalbrandDbContext.SaveChanges();


        }
    }
}