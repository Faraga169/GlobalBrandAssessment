using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Repositories;

namespace GlobalBrandAssessment.BL.Services
{
   public  class CommentService:ICommentService
    {
        private readonly ICommentRepository commentRepository;

        public CommentService(ICommentRepository commentRepository)
        {
            this.commentRepository = commentRepository;
        }

        public int Add(Comment Comment)
        {
            if (Comment == null)
            {
                return 0;
            }
            return commentRepository.Add(Comment);
        }
    }
}
