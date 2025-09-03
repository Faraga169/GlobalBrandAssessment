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


       /*Navigation property*/
        public Tasks Task { get; set; } = null!;
        public Employee Employee { get; set; } = null!;

        /*Foreign keys*/
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        [ForeignKey("Task")]
        public int TaskId { get; set; }


    }
}
