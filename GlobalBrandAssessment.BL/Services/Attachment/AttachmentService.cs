using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GlobalBrandAssessment.BL.DTOS.AttachmentDTO;
using GlobalBrandAssessment.BL.DTOS.CommentDTO;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Repositories;

namespace GlobalBrandAssessment.BL.Services
{
    public class AttachmentService : IAttachmentService
    {
        private readonly IAttachmentRepository attachmentRepository;
        private readonly IMapper mapper;

        public AttachmentService(IAttachmentRepository attachmentRepository,IMapper mapper)
        {
            this.attachmentRepository = attachmentRepository;
            this.mapper =mapper;
        }
        public int AddOrUpdate(AddAndUpdateAttachmentDTO attachment)
        {
            var result = mapper.Map<AddAndUpdateAttachmentDTO, Attachment>(attachment);

            return attachmentRepository.AddOrUpdate(result);
        }
    }
}
