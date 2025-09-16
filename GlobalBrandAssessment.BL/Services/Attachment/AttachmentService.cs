using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GlobalBrandAssessment.BL.DTOS.AttachmentDTO;
using GlobalBrandAssessment.BL.DTOS.CommentDTO;
using GlobalBrandAssessment.BL.Services.Generic;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Repositories;
using GlobalBrandAssessment.DAL.Repositories.Generic;
using GlobalBrandAssessment.DAL.UnitofWork;
using GlobalBrandAssessment.GlobalBrandDbContext;

namespace GlobalBrandAssessment.BL.Services
{
    public class AttachmentService : GenericService<Attachment,AddAndUpdateAttachmentDTO>, IAttachmentService
    {
        private readonly IUnitofWork unitofWork;
     
        private readonly IMapper mapper;

        public AttachmentService(IUnitofWork unitofWork,IMapper mapper): base(unitofWork,mapper)
        {
            this.unitofWork = unitofWork;
           
            this.mapper =mapper;
        }

      

        public async Task<Attachment?> GetByTaskIdAsync(int taskId)
        {
            
         var result= await unitofWork.attachmentRepository.GetByTaskId(taskId);
           return result is not null ? result : null;
        }

        public async Task<int> DeleteAsync(int id)
        {
            var attachment = await unitofWork.attachmentRepository.GetByTaskId(id);
            if (attachment is null)
                return 0;

            await unitofWork.attachmentRepository.DeleteAsync(attachment);
            var result = await unitofWork.CompleteAsync();
            return result > 0 ? result : 0;
        }

    }
}
