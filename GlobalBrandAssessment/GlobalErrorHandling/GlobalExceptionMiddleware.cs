using Serilog;

namespace GlobalBrandAssessment.PL.GlobalErrorHandling
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, IWebHostEnvironment env)
        {
            _next = next;
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var userName = context.User.Identity.Name;
            

                var controller = context.GetRouteValue("controller")?.ToString() ;
                var action = context.GetRouteValue("action")?.ToString();
                var path = context.Request.Path;

              
                Log.ForContext("UserName", userName)
                   .ForContext("Controller", controller)
                   .ForContext("ActionType", action)
                   .Error(ex, "Unhandled exception in {Controller}/{Action} for user {UserName} at path {Path}", controller, action, userName, path);
            




                if (IsAjaxRequest(context.Request))
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(new
                    {
                        success = false,
                        message = _env.IsDevelopment() ? ex.Message : "An error occurred. Please try again later."
                    });
                }
                else
                {
                    context.Response.Redirect("/Error");
                }
            }
        }

          

        private bool IsAjaxRequest(HttpRequest request)
        {
            return request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }

    }
}
