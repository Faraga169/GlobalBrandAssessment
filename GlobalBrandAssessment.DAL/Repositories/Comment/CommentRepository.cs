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

        public int Add(Comment comment)
        {
         globalbrandDbContext.Comments.Add(comment);
         return globalbrandDbContext.SaveChanges();


        }
    }
}
