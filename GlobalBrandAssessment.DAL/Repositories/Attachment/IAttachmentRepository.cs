using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;

namespace GlobalBrandAssessment.DAL.Repositories
{
    public interface IAttachmentRepository
    {
        public int Add(Data.Models.Attachment attachment);
    }
}
