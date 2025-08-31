using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;
using Microsoft.AspNetCore.Http;

namespace GlobalBrandAssessment.BL.DTOS.AttachmentDTO
{
    public class AddAndUpdateAttachmentDTO
    {
       
        public int AttachmentId { get; set; }
        public int TaskId { get; set; }

        [Display(Name = "Attachment")]
        public string? FilePath { get; set; }

        public int UploadedById { get; set; }
    }
}
