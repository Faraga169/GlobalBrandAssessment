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
using GlobalBrandAssessment.GlobalBrandDbContext;

namespace GlobalBrandAssessment.BL.Services
{
   public  class CommentService:GenericService<Comment>, ICommentService
    {
        private readonly ICommentRepository commentRepository;
        private readonly IMapper mapper;

        public CommentService(ICommentRepository commentRepository,IMapper mapper) : base(commentRepository)
        {
            this.commentRepository = commentRepository;
            this.mapper = mapper;
        }
        public async Task<int> AddAsync(AddAndUpdateCommentDTO comment)
        {
            if (comment == null)
            {
                return 0;
            }
            return await commentRepository.AddAsync(mapper.Map<AddAndUpdateCommentDTO,Comment>(comment));
        }

        public async Task<int> UpdateAsync(AddAndUpdateCommentDTO comment)
        {
            if (comment == null)
            {
                return 0;
            }
            return await commentRepository.UpdateAsync(mapper.Map<AddAndUpdateCommentDTO, Comment>(comment));
        }

        public async Task<Comment?> GetByTaskIdAsync(int taskId)
        {
            return await commentRepository.GetByTaskId(taskId);
        }


    }
}
