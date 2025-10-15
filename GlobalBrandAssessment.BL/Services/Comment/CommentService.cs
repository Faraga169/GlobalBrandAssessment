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
            var result = await unitofWork.Repository<ICommentRepository,Comment>().GetByTaskId(taskId);
            return result;
        }

        public async Task<int> DeleteAsync(int id)
        {
            var comment = await unitofWork.Repository<ICommentRepository, Comment>().GetByTaskId(id);
            if (comment is null)
                return 0;

            await unitofWork.Repository<ICommentRepository, Comment>().DeleteAsync(comment);
            var result = await unitofWork.CompleteAsync();
            return result > 0 ? result : 0;
        }
    }
}
