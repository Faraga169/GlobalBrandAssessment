using GlobalBrandAssessment.BL.Profiles.AttachmentProfile;
using GlobalBrandAssessment.BL.Profiles.CommentProfile;
using GlobalBrandAssessment.BL.Profiles.DepartmentProfile;
using GlobalBrandAssessment.BL.Profiles.ManagerProfile;
using GlobalBrandAssessment.BL.Profiles.TaskProfile;
using GlobalBrandAssessment.BL.Services;
using GlobalBrandAssessment.BL.Services.Generic;
using GlobalBrandAssessment.BL.Services.Manager;
using GlobalBrandAssessment.BL.Services.Task;
using GlobalBrandAssessment.DAL.Data;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Repositories;
using GlobalBrandAssessment.DAL.Repositories.Attachment;
using GlobalBrandAssessment.DAL.Repositories.Generic;
using GlobalBrandAssessment.DAL.UnitofWork;
using GlobalBrandAssessment.GlobalBrandDbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GlobalBrandAssessment
{
    public class Program
    {
        public static  void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            #region   Add services to the container.
            builder.Services.AddControllersWithViews(options=>options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // مدة الجلسة
                options.Cookie.HttpOnly = true; // حماية الكوكيز
                options.Cookie.IsEssential = true; // ضروري حتى مع GDPR
            });
            builder.Services.AddDbContext<GlobalbrandDbContext>(options =>   
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
                //options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"])
                //options.UseSqlServer(builder.Configuration.GetSection("ConnectionStrings")[key: "DefaultConnection"])
                ); //Register To Service In DI Container

            builder.Services.AddDistributedMemoryCache();

            //builder.Services.AddScoped<IManagerRepository, ManagerRepository>();
            builder.Services.AddScoped<IManagerService, ManagerService>();
            //builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();
            //builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            builder.Services.AddScoped<IDepartmentService, DepartmentService>();
            //builder.Services.AddScoped<ITaskRepository, TaskRepository>();
            builder.Services.AddScoped<ITaskService, TaskService>();
           // builder.Services.AddScoped<IAttachmentRepository, AttachmentRepository>();
            builder.Services.AddScoped<IAttachmentService, AttachmentService>();
            //builder.Services.AddScoped<ICommentRepository, CommentRepository>();
            builder.Services.AddScoped<ICommentService, CommentService>();
            //builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IUnitofWork, UnitOfWork>();
            //builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped(typeof(IGenericService<,>), typeof(GenericService<,>));
            builder.Services.AddAutoMapper(m => m.AddProfile(new ManagerMapping()));
            builder.Services.AddAutoMapper(m => m.AddProfile(new DepartmentMapping()));
            builder.Services.AddAutoMapper(m => m.AddProfile(new TaskMapping()));
            builder.Services.AddAutoMapper(m => m.AddProfile(new AttachmentMapping()));
            builder.Services.AddAutoMapper(m => m.AddProfile(new CommentMapping()));
           
            #endregion

            var app = builder.Build();

           
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
          
            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();
          
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Login}/{action=Index}/{id:int?}");
 
            //using (var scope = app.Services.CreateScope())
            //{
            //    var services = scope.ServiceProvider;
            //    var userManager = services.GetRequiredService<UserManager<User>>(); 
            //    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            //    await ApplicationDbContextSeed.SeedUsersAndRolesAsync(userManager, roleManager);
            //}
           

            app.Run();
        }
    }
}
