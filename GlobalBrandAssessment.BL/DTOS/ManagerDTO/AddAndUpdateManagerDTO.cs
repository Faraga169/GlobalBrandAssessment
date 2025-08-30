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

namespace GlobalBrandAssessment.BL.DTOS.ManagerDTO
{
    public class AddAndUpdateManagerDTO
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
        public decimal? Salary { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$#!%*?&])[A-Za-z\d@$#!%*?&]{6,12}$", ErrorMessage = "Password must be 6-12 characters long, contain at least one uppercase letter, one lowercase letter, and one digit and special character")]
        public string Password { get; set; } = null!;

        [DisplayName("Image")]
        public string? ImageURL { get; set; }

        public IFormFile? Image { get; set; } = null!;


        [DisplayName("Department")]
        [Required(ErrorMessage = "Department is required.")]
        public int? DeptId { get; set; }

        public int? ManagerId { get; set; }
    }
}
