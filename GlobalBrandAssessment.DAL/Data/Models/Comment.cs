using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalBrandAssessment.DAL.Data.Models
{
    public class Comment
    {
        public int CommentId { get; set; }

        [Display(Name = "Comment")]
        public string? Content { get; set; }        
        public DateTime CreatedAt { get; set; }= DateTime.Now;


        [ForeignKey("Task")]
        public int TaskId { get; set; }
        public Tasks Task { get; set; }


        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

      
    }
}
