using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GlobalBrandAssessment.BL.DTOS.AttachmentDTO;

namespace GlobalBrandAssessment.BL.Profiles.AttachmentProfile
{
    public class AttachmentMapping:Profile
    {
        public AttachmentMapping()
        {
            CreateMap<AddAndUpdateAttachmentDTO, DAL.Data.Models.Attachment>().ReverseMap();
            CreateMap<PL.ViewModels.TaskEditViewModel,AddAndUpdateAttachmentDTO>().ForMember(tdest => tdest.TaskId, option => option.MapFrom(tsrc => tsrc.Id))
                .ForMember(tdest=>tdest.FilePath,option=>option.MapFrom(tsrc=>tsrc.Attachment.FileName));

        }
    }
}
