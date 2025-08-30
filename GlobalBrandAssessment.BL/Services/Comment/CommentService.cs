using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GlobalBrandAssessment.BL.DTOS.CommentDTO;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Repositories;

namespace GlobalBrandAssessment.BL.Services
{
   public  class CommentService:ICommentService
    {
        private readonly ICommentRepository commentRepository;
        private readonly IMapper mapper;

        public CommentService(ICommentRepository commentRepository,IMapper mapper)
        {
            this.commentRepository = commentRepository;
            this.mapper = mapper;
        }

        public int AddOrUpdate(AddAndUpdateCommentDTO Comment)
        {
            var result = mapper.Map<AddAndUpdateCommentDTO, Comment>(Comment);
          
          
            return commentRepository.AddOrUpdate(result);
        }

       
    }
}
