using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GlobalBrandAssessment.BL.DTOS.ManagerDTO;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Data.Models.Common;

namespace GlobalBrandAssessment.BL.Profiles.ManagerProfile
{
    public class ManagerMapping:Profile
    {
        public ManagerMapping()
        {
            CreateMap<AddAndUpdateManagerDTO, Employee>().ForMember(dest=>dest.Roles,option=>option.MapFrom(src=>Enum.Parse<Role>(src.Role)));
            CreateMap<Employee,AddAndUpdateManagerDTO>().ForMember(dest => dest.Role, option => option.MapFrom(src => src.Roles.ToString()));
            CreateMap<Employee,GetAllAndSearchManagerDTO>().ForMember(tdest=>tdest.Department,option=>option.MapFrom(tsource=>tsource.Department != null ? tsource.Department.Name : null)).ForMember(dest => dest.Role, option => option.MapFrom(src => src.Roles.ToString()));
            CreateMap<AddAndUpdateManagerDTO,User>().ForMember(tdest=>tdest.UserName,option=>option.MapFrom(tsrc=>tsrc.FirstName+tsrc.LastName));
        }
    }
}
