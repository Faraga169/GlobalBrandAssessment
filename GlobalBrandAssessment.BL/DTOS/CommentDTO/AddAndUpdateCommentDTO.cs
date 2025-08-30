using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;

namespace GlobalBrandAssessment.BL.DTOS.CommentDTO
{
    public class AddAndUpdateCommentDTO
    {
        public int TaskId { get; set; }


        [Display(Name = "Comment")]
        public string? Content { get; set; }

        public int UserId { get; set; }
    }
}
