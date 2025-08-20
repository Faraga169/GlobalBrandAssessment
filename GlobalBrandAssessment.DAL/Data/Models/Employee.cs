using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace GlobalBrandAssessment.DAL.Data.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Employee First Name is required.")]
        [StringLength(30, ErrorMessage = "Employee First Name cannot be longer than 30 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Employee Name can only contain letters and spaces.")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Employee Last Name is required.")]
        [StringLength(30, ErrorMessage = "Employee Last Name cannot be longer than 30 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Employee Last Name can only contain letters and spaces.")]
        public string LastName { get; set; } = null!;

        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue, ErrorMessage = "Salary must be a positive number.")]
        [Required(ErrorMessage = "Salary is required.")]
        public decimal?  Salary { get; set; }

        [DisplayName("Image")]
        public string? ImageURL { get; set; }

        [NotMapped]
        public IFormFile? Image { get; set; } = null!;

        [NotMapped]
        [DisplayName("Manager Name")]
        public string FullName {

            get { 
            return $"{FirstName} {LastName}";
            }
                
                }

        /*Navigation Property*/
        public virtual Employee? Manager { get; set; } = null!;

        public virtual Department? Department { get; set; } = null!;

        public virtual User? User { get; set; } = null!;
        public virtual ICollection<Tasks> Tasks { get; set; } = new HashSet<Tasks>();



        [ForeignKey("Manager")]
        [DisplayName("Manager")]
        public int? ManagerId { get; set; }

        [ForeignKey("Department")]
        [DisplayName("Department")]
        [Required(ErrorMessage = "Department is required.")]
        public int? DeptId { get; set; }




    }
}
