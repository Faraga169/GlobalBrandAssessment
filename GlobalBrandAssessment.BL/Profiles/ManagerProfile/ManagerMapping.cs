using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GlobalBrandAssessment.BL.DTOS.ManagerDTO;
using GlobalBrandAssessment.DAL.Data.Models;

namespace GlobalBrandAssessment.BL.Profiles.ManagerProfile
{
    public class ManagerMapping:Profile
    {
        public ManagerMapping()
        {
            CreateMap<AddAndUpdateManagerDTO, Employee>().ReverseMap();
            CreateMap<Employee,GetAllAndSearchManagerDTO>().ForMember(tdest=>tdest.Department,option=>option.MapFrom(tsource=>tsource.Department.Name));
            CreateMap<AddAndUpdateManagerDTO,User>().ForMember(tdest=>tdest.UserName,option=>option.MapFrom(tsrc=>tsrc.FirstName));
        }
    }
}
