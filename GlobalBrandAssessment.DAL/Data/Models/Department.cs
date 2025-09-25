using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlobalBrandAssessment.DAL.Data.Models
{
    public class Department
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Department Name is required.")]
        [StringLength(50, ErrorMessage = "Department Name cannot be longer than 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Department Name can only contain letters and spaces.")]
        [DisplayName("Department Name")]
        public string Name { get; set; } = null!;

        /*Navigation property*/
       
        public virtual ICollection<Employee> Employees { get; set; } = new HashSet<Employee>();
      
        public virtual Employee? Manager { get; set; } = null!;


        [DataType(DataType.Currency)]
        [NotMapped]
        [DisplayName("TotalSalary")]
        public decimal SumSalary
        {

            get
            {

                return Employees?.Where(E => E.DeptId == Id && E.Id != ManagerId).Sum(E => E.Salary) ?? 0;

            }

        }

        [NotMapped]
        [DisplayName("Number of Employees")]
        public int EmployeeCount
        {
            get
            {
                return Employees?.Count(e => e.Id != ManagerId) ?? 0;
            }
        }

        //[NotMapped]
        //[Required(ErrorMessage = "Description is required.")]
        //public string Description { get; set; } = null!;

        /*Foreign key*/
        [ForeignKey("Manager")]
        [DisplayName("Manager")]
        //[Required(ErrorMessage = "Manager is required.")]
        public int? ManagerId { get; set; }


    }
}
