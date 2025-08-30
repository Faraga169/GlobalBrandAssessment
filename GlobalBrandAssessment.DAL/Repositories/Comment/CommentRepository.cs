using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.GlobalBrandDbContext;

namespace GlobalBrandAssessment.DAL.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly GlobalbrandDbContext globalbrandDbContext;

        public CommentRepository(GlobalbrandDbContext globalbrandDbContext)
        {
            this.globalbrandDbContext = globalbrandDbContext;
        }

        public int AddOrUpdate(Comment comment)
        {
            var existingComment = globalbrandDbContext.Comments
                .FirstOrDefault(c => c.TaskId == comment.TaskId);

            if (existingComment != null)
            {
               
                existingComment.Content = comment.Content;
                existingComment.UserId= comment.UserId; 
                globalbrandDbContext.Comments.Update(existingComment);
            }
            else
            {
               
                globalbrandDbContext.Comments.Add(comment);
            }

            return globalbrandDbContext.SaveChanges();
        }
    }
}
