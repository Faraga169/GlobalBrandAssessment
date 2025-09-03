using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;

namespace GlobalBrandAssessment.BL.DTOS.DepartmentDTO
{
    public class AddAndUpdateDepartmentDTO
    {
        
        public int Id { get; set; }

        [Required(ErrorMessage = "Department Name is required.")]
        [StringLength(50, ErrorMessage = "Department Name cannot be longer than 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Department Name can only contain letters and spaces.")]
        [DisplayName("Department Name")]
        public string Name { get; set; } = null!;

       

        [DisplayName("Manager")]
        public int? ManagerId { get; set; }
    }
}
