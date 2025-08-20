using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlobalBrandAssessment.DAL.Data.Models
{
    public class Tasks
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Task Title is required.")]
        [StringLength(50, ErrorMessage = "Task Title cannot be longer than 50 characters.")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Task Description is required.")]
        [StringLength(100, ErrorMessage = "Task Description cannot be longer than 100 characters.")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Task Status is required.")]
        [RegularExpression(@"^(Pending|InProgress|Completed)$", ErrorMessage = "Task Status must be either 'Pending', 'InProgress', or 'Completed'.")]
        public string Status { get; set; } = "Pending"; // Default status

        /*Navigation property*/
        public virtual Employee? AssignedEmployee { get; set; } = null!;
        public virtual Comment Comments { get; set; }

        public virtual Attachment Attachments{ get; set; }


        [ForeignKey("AssignedEmployee")]
        [DisplayName("Employee Name")]
        [Required(ErrorMessage = "Employee is required.")]
        public int? EmployeeId { get; set; }

    }
}
