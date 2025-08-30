using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.BL.DTOS.CommentDTO;
using GlobalBrandAssessment.DAL.Data.Models;

namespace GlobalBrandAssessment.BL.Services
{
    public interface ICommentService
    {
        public int AddOrUpdate(AddAndUpdateCommentDTO Comment);
       
    }
}
