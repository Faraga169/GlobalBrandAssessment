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
using GlobalBrandAssessment.GlobalBrandDbContext;

namespace GlobalBrandAssessment.BL.Services
{
    public class AttachmentService : GenericService<Attachment>, IAttachmentService
    {
    private readonly IAttachmentRepository attachmentRepository;
        private readonly IMapper mapper;

        public AttachmentService(IAttachmentRepository attachmentRepository,IMapper mapper): base(attachmentRepository)
        {
            this.attachmentRepository = attachmentRepository;
            this.mapper =mapper;
        }
       

        public async Task<int> AddAsync(AddAndUpdateAttachmentDTO attachment)
        {
            if (attachment == null)
                return 0;

            return await attachmentRepository.AddAsync(mapper.Map<AddAndUpdateAttachmentDTO, Attachment>(attachment));

        }

        public async Task<int> UpdateAsync(AddAndUpdateAttachmentDTO attachment)
        {
            if(attachment==null)
                return 0;
            return await attachmentRepository.UpdateAsync(mapper.Map<AddAndUpdateAttachmentDTO, Attachment>(attachment));
        }

        public async Task<DAL.Data.Models.Attachment?> GetByTaskIdAsync(int taskId)
        {
            return await attachmentRepository.GetByTaskId(taskId);
        }
    }
}
