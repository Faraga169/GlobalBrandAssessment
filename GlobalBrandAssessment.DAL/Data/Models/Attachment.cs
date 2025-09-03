using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GlobalBrandAssessment.DAL.Data.Models
{
    public class Attachment
    {
        public int AttachmentId { get; set; }

        [Display(Name ="Attachment")]
        public string? FilePath { get; set; }        
        public DateTime UploadedAt { get; set; } = DateTime.Now;
        [NotMapped]
        public IFormFile? File { get; set; }

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
