using GlobalBrandAssessment.DAL.Data.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace GlobalBrandAssessment.PL.ViewModels
{
    public class CreateTaskViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Task Title is required.")]
        [StringLength(50, ErrorMessage = "Task Title cannot be longer than 50 characters.")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Task Description is required.")]
        [StringLength(100, ErrorMessage = "Task Description cannot be longer than 100 characters.")]
        public string Description { get; set; } = null!;


        [ForeignKey("AssignedEmployee")]
        [DisplayName("Employee Name")]
        [Required(ErrorMessage = "Employee is required.")]
        public int? EmployeeId { get; set; }




    }
}
