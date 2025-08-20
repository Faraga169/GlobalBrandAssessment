using GlobalBrandAssessment.DAL.Data.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace GlobalBrandAssessment.PL.ViewModels
{
    public class CreateDepartmentViewModel
    {

        [Required(ErrorMessage = "Department Name is required.")]
        [StringLength(50, ErrorMessage = "Department Name cannot be longer than 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Department Name can only contain letters and spaces.")]
        [DisplayName("Department Name")]
        public string Name { get; set; } = null!;

        [ForeignKey("Manager")]
        [DisplayName("Manager")]
        [Required(ErrorMessage = "Manager is required.")]
        public int? ManagerId { get; set; }
    }
}
