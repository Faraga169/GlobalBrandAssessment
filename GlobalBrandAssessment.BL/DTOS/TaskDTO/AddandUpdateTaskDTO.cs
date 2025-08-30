using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;

namespace GlobalBrandAssessment.BL.DTOS.TaskDTO
{
    public class AddandUpdateTaskDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Task Title is required.")]
        [StringLength(50, ErrorMessage = "Task Title cannot be longer than 50 characters.")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Task Description is required.")]
        [StringLength(100, ErrorMessage = "Task Description cannot be longer than 100 characters.")]
        public string Description { get; set; } = null!;

        public string? Status { get; set; } = "Pending";

        public string? Content { get; set; }

        public string? FilePath { get; set; }
        [DisplayName("Employee Name")]
        [Required(ErrorMessage = "Employee is required.")]
        public int? EmployeeId { get; set; }
    }
}
