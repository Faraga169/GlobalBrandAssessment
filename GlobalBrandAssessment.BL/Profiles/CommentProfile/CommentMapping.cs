using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GlobalBrandAssessment.BL.DTOS.CommentDTO;
using GlobalBrandAssessment.PL.ViewModels;

namespace GlobalBrandAssessment.BL.Profiles.CommentProfile
{
    public class CommentMapping:Profile
    {
        public CommentMapping()
        {
            CreateMap<AddAndUpdateCommentDTO, DAL.Data.Models.Comment>().ReverseMap();
            CreateMap<TaskEditViewModel, AddAndUpdateCommentDTO>().ForMember(tdest=>tdest.TaskId,option=>option.MapFrom(tsrc=>tsrc.Id));
        }
    }
}
