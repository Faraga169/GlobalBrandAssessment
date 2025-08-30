using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GlobalBrandAssessment.BL.DTOS.TaskDTO;
using GlobalBrandAssessment.PL.ViewModels;

namespace GlobalBrandAssessment.BL.Profiles.TaskProfile
{
    public class TaskMapping:Profile
    {
        public TaskMapping()
        {
            CreateMap<AddandUpdateTaskDTO, DAL.Data.Models.Tasks>().ReverseMap().ForMember(tdest=>tdest.Content,option=>option.MapFrom(tsrc=>tsrc.Comments.Content))
                .ForMember(tdest => tdest.FilePath, option => option.MapFrom(tsrc => tsrc.Attachments.FilePath));
            CreateMap<DAL.Data.Models.Tasks, GetAllandSearchTaskDTO>().ForMember(tdest => tdest.FirstName, option => option.MapFrom(tsource => tsource.AssignedEmployee.FirstName))
                .ForMember(tdest => tdest.LastName, option => option.MapFrom(tsource => tsource.AssignedEmployee.LastName))
                .ForMember(tdest => tdest.Content, option => option.MapFrom(tsrc => tsrc.Comments.Content))
                .ForMember(tdest => tdest.FilePath, option => option.MapFrom(tsrc => tsrc.Attachments.FilePath)); 
            CreateMap<AddandUpdateTaskDTO, TaskEditViewModel>().ReverseMap();
        }
    }

}
