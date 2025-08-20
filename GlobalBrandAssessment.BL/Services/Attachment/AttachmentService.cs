using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Repositories;

namespace GlobalBrandAssessment.BL.Services
{
    public class AttachmentService : IAttachmentService
    {
        private readonly IAttachmentRepository attachmentRepository;

        public AttachmentService(IAttachmentRepository attachmentRepository)
        {
            this.attachmentRepository = attachmentRepository;
        }
        public int Add(Attachment attachment)
        {
            if(attachment==null)
                return 0;
            return attachmentRepository.Add(attachment);
        }
    }
}
