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
    public class GetAllandSearchDepartmentDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        [DisplayName("Manager Name")]
        public string Manager { get; set; } = null!;

        public virtual ICollection<Employee> Employees { get; set; } = new HashSet<Employee>();

        [DataType(DataType.Currency)]
        [DisplayName("TotalSalary")]
        public decimal SumSalary
        {

            get
            {

                return Employees?.Where(E => E.DeptId == Id && E.Id != ManagerId).Sum(E => E.Salary) ?? 0;

            }

        }

        [DisplayName("Number of Employees")]
        public int EmployeeCount
        {
            get
            {
                return Employees?.Count(e => e.Id != ManagerId) ?? 0;
            }
        }

        [DisplayName("Manager")]
        public int? ManagerId { get; set; }

        //[Required(ErrorMessage = "Description is required.")]
        //public string Description { get; set; } = null!;
    }
}
