using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.BL.DTOS.AttachmentDTO;
using GlobalBrandAssessment.BL.DTOS.CommentDTO;
using GlobalBrandAssessment.DAL.Data.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GlobalBrandAssessment.BL.Services
{
    public interface IAttachmentService
    {
        public int AddOrUpdate(AddAndUpdateAttachmentDTO Comment);
    }
}
