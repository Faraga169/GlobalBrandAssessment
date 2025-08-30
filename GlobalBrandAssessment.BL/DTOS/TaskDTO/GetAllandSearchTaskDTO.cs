using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;
using Microsoft.AspNetCore.Http;

namespace GlobalBrandAssessment.BL.DTOS.TaskDTO
{
   public class GetAllandSearchTaskDTO
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;
        public string Status { get; set; } = "Pending"; 

        public string FirstName{ get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string? FilePath { get; set; } = null!;

        public string? Content { get; set; } = null!;

       




    }
}
