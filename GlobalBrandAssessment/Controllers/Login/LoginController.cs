using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.GlobalBrandDbContext;
using GlobalBrandAssessment.PL.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace GlobalBrandAssessment.PL.Controllers.Login
{
    public class LoginController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public LoginController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            Log.ForContext("ActionType", "AccessDeniedPage")
               .ForContext("Controller", "Login")
               .Warning("Unauthorized access attempt detected.");
            return View();
        }

        [HttpGet]
        public IActionResult Index()
        {
            Log.ForContext("ActionType", "AccessLoginPage")
               .ForContext("Controller", "Login")
               .Information("Login page accessed.");
            return View();
        }

        [HttpPost]
        public IActionResult Index(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                Log.ForContext("ActionType", "LoginAttemptStarted")
                   .ForContext("Controller", "Login")
                   .Information("Login attempt initiated for user: {UserName}", loginViewModel.UserName);

                var user = userManager.FindByNameAsync(loginViewModel.UserName).Result;
                if (user == null)
                {
                    Log.ForContext("ActionType", "LoginFailed_UserNotFound")
                       .ForContext("Controller", "Login")
                       .Warning("Login failed: Username {UserName} does not exist.", loginViewModel.UserName);

                    ModelState.AddModelError("", "Username does not exist");
                    return View(loginViewModel);
                }

                var passwordValid = userManager.CheckPasswordAsync(user, loginViewModel.Password).Result;
                if (!passwordValid)
                {
                    Log.ForContext("ActionType", "LoginFailed_InvalidPassword")
                       .ForContext("Controller", "Login")
                       .Warning("Login failed: Invalid password for user {UserName}.", loginViewModel.UserName);

                    ModelState.AddModelError("", "Invalid password");
                    return View(loginViewModel);
                }

                var result = signInManager.PasswordSignInAsync(user, loginViewModel.Password, loginViewModel.RememberMe, lockoutOnFailure: false).Result;

                if (result.IsNotAllowed)
                {
                    Log.ForContext("ActionType", "LoginBlocked_NotAllowed")
                       .ForContext("Controller", "Login")
                       .Warning("Login blocked: User {UserName} is not allowed to sign in.", loginViewModel.UserName);

                    ModelState.AddModelError("", "You are not allowed to login");
                    return View(loginViewModel);
                }

                if (result.IsLockedOut)
                {
                    Log.ForContext("ActionType", "LoginBlocked_AccountLocked")
                       .ForContext("Controller", "Login")
                       .Warning("Login blocked: User {UserName} account is locked.", loginViewModel.UserName);

                    ModelState.AddModelError("", "You are locked out");
                    return View(loginViewModel);
                }

                if (result.Succeeded)
                {
                    var userRole = userManager.GetRolesAsync(user).Result.FirstOrDefault();

                    Log.ForContext("ActionType", "LoginSuccessful")
                       .ForContext("Controller", "Login")
                       .Information("User {UserName} logged in successfully with role '{UserRole}'.", loginViewModel.UserName, userRole);

                    return userRole switch
                    {
                        "Manager" => RedirectToAction("Index", "Manager"),
                        "Employee" => RedirectToAction("Index", "Employee"),
                        "Admin" => RedirectToAction("Index", "Admin"),
                        _ => RedirectToAction("Index", "Login")
                    };
                }

                Log.ForContext("ActionType", "LoginFailed_UnknownReason")
                   .ForContext("Controller", "Login")
                   .Warning("Login failed for user {UserName} due to unknown reason.", loginViewModel.UserName);
            }
            else
            {
                Log.ForContext("ActionType", "LoginValidationFailed")
                   .ForContext("Controller", "Login")
                   .Warning("Login form validation failed for user input.");
            }

            return View(loginViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var userName = User.Identity?.Name ?? "Unknown";
            await signInManager.SignOutAsync();

            Log.ForContext("UserName", userName)
               .ForContext("ActionType", "UserLogout")
               .ForContext("Controller", "Login")
               .Information("User {UserName} logged out successfully.", userName);

            return RedirectToAction("Index", "Login");
        }
    }
}
