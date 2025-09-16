using System.ComponentModel.DataAnnotations;

namespace GlobalBrandAssessment.PL.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "User Name is required.")]
        [StringLength(100, ErrorMessage = "User Name cannot be longer than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "User Name can only contain letters and numbers.")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#]{6,}$", ErrorMessage = "Password must be 6-12 characters long, contain at least one uppercase letter, one lowercase letter, and one digit and special character")]
        public string Password { get; set; } = null!;

        public bool RememberMe { get; set; }    
    }
}
