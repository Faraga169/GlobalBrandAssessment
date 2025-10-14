using Microsoft.AspNetCore.Mvc;

namespace GlobalBrandAssessment.PL.Controllers.Error
{
    public class ErrorController : Controller
    {
        [Route("Error")]
        public IActionResult Index()
        {
            return View("Error");
        }
    }
}
