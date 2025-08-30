using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GlobalBrandAssessment.BL.DTOS.DepartmentDTO;
using GlobalBrandAssessment.DAL.Data.Models;

namespace GlobalBrandAssessment.BL.Profiles.DepartmentProfile
{
    public class DepartmentMapping : Profile
    {
        public DepartmentMapping()
        {
            CreateMap<AddAndUpdateDepartmentDTO, Department>().ReverseMap();
            CreateMap<Department, GetAllandSearchDepartmentDTO>().ForMember(tdest => tdest.Manager, option => option.MapFrom(tsource => tsource.Manager.FullName));
        }

    }
}
