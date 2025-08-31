using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.BL.DTOS.AttachmentDTO;
using GlobalBrandAssessment.BL.DTOS.CommentDTO;
using GlobalBrandAssessment.BL.Services.Generic;
using GlobalBrandAssessment.DAL.Data.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GlobalBrandAssessment.BL.Services
{
    public interface IAttachmentService:IGenericService<AddAndUpdateAttachmentDTO>
    {
        public Task<DAL.Data.Models.Attachment?> GetByTaskIdAsync(int taskId);
    }
}
