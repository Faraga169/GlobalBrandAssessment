using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GlobalBrandAssessment.BL.DTOS.CommentDTO;
using GlobalBrandAssessment.BL.Services.Generic;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Repositories;
using GlobalBrandAssessment.DAL.Repositories.Generic;
using GlobalBrandAssessment.DAL.UnitofWork;
using GlobalBrandAssessment.GlobalBrandDbContext;

namespace GlobalBrandAssessment.BL.Services
{
   public  class CommentService:GenericService<Comment,AddAndUpdateCommentDTO>, ICommentService
    {
        private readonly IUnitofWork unitofWork;
      

        private readonly IMapper mapper;

        public CommentService(IUnitofWork unitofWork,IMapper mapper) : base(unitofWork,mapper)
        {
            this.unitofWork = unitofWork;
            
            
            this.mapper = mapper;
        }
       

        public async Task<Comment?> GetByTaskIdAsync(int taskId)
        {
            var result = await unitofWork.commentRepository.GetByTaskId(taskId);
            return result;
        }


    }
}
