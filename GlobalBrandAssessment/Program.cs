using System.Collections.ObjectModel;
using System.Data;
using GlobalBrandAssessment.BL.Profiles.AttachmentProfile;
using GlobalBrandAssessment.BL.Profiles.CommentProfile;
using GlobalBrandAssessment.BL.Profiles.DepartmentProfile;
using GlobalBrandAssessment.BL.Profiles.ManagerProfile;
using GlobalBrandAssessment.BL.Profiles.TaskProfile;
using GlobalBrandAssessment.BL.Services;
using GlobalBrandAssessment.BL.Services.Generic;
using GlobalBrandAssessment.BL.Services.Manager;
using GlobalBrandAssessment.BL.Services.Task;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Repositories;
using GlobalBrandAssessment.DAL.Repositories.Attachment;
using GlobalBrandAssessment.DAL.Repositories.Generic;
using GlobalBrandAssessment.DAL.Seeding;
using GlobalBrandAssessment.DAL.UnitofWork;
using GlobalBrandAssessment.GlobalBrandDbContext;
using GlobalBrandAssessment.PL.GlobalErrorHandling;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

namespace GlobalBrandAssessment
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()

   .MinimumLevel.Information() 
.MinimumLevel.Override("Microsoft", LogEventLevel.Error)
.MinimumLevel.Override("System", LogEventLevel.Error)

                .WriteTo.Console()
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
                .WriteTo.MSSqlServer
                (
                    connectionString: "Server=.;Database=GlobalBrandAssessment;Trusted_Connection=True;TrustServerCertificate=True",
                    sinkOptions: new MSSqlServerSinkOptions { TableName = "AuditLogs", AutoCreateSqlTable = true }
                ,
                  columnOptions: new ColumnOptions() {

                  AdditionalColumns= new Collection<SqlColumn>
        {
            new SqlColumn("UserName", SqlDbType.NVarChar, dataLength: 100),
            new SqlColumn("ActionType", SqlDbType.NVarChar, dataLength: 100),
            new SqlColumn("Controller", SqlDbType.NVarChar, dataLength: 100)
        }
                  }
        ).CreateLogger();



            var builder = WebApplication.CreateBuilder(args);

            #region Add services to the container.
            // Replace default logger with Serilog
            builder.Host.UseSerilog();

            builder.Services.AddControllersWithViews(options =>
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));

            

            builder.Services.AddDbContext<GlobalbrandDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddScoped<IManagerService, ManagerService>();
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();
            builder.Services.AddScoped<IDepartmentService, DepartmentService>();
            builder.Services.AddScoped<ITaskService, TaskService>();
            builder.Services.AddScoped<IAttachmentService, AttachmentService>();
            builder.Services.AddScoped<ICommentService, CommentService>();
            builder.Services.AddScoped<IManagerRepository, ManagerRepository>();
            builder.Services.AddScoped<IAttachmentRepository, AttachmentRepository>();
            builder.Services.AddScoped<ICommentRepository, CommentRepository>();
            builder.Services.AddScoped<ITaskRepository, TaskRepository>();
            builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<IUnitofWork, UnitOfWork>();
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped(typeof(IGenericService<,>), typeof(GenericService<,>));

            builder.Services.AddAutoMapper(m => m.AddProfile(new ManagerMapping()));
            builder.Services.AddAutoMapper(m => m.AddProfile(new DepartmentMapping()));
            builder.Services.AddAutoMapper(m => m.AddProfile(new TaskMapping()));
            builder.Services.AddAutoMapper(m => m.AddProfile(new AttachmentMapping()));
            builder.Services.AddAutoMapper(m => m.AddProfile(new CommentMapping()));


            //Allow DI UserManager and RoleManager and SignInManager
            builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<GlobalbrandDbContext>().AddDefaultTokenProviders();

            // This Service for controllement of Configuration of token
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Login/Index";
                options.AccessDeniedPath = "/Login/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(45);
            });

        

           
            #endregion

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.UseStaticFiles();
            app.Use(async (context, next) =>
            {
                context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                context.Response.Headers["Pragma"] = "no-cache";
                context.Response.Headers["Expires"] = "0";
                await next();
            });
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
          
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Login}/{action=Index}/{id:int?}");

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<GlobalbrandDbContext>();
                await context.Database.MigrateAsync(); 

                var userManager = services.GetRequiredService<UserManager<User>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var globalbrandDbContext = services.GetRequiredService<GlobalbrandDbContext>();
             
                if (!await userManager.Users.AnyAsync())
                {
                    await ApplicationDbContextSeed.SeedUsersAndRolesAsync(userManager, roleManager, globalbrandDbContext);
                }
            }

            app.Run();
        }
    }
}
