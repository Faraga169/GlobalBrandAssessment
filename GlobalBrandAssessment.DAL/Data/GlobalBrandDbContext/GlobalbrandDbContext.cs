using System.Data;
using GlobalBrandAssessment.DAL.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tasks = GlobalBrandAssessment.DAL.Data.Models.Tasks;

namespace GlobalBrandAssessment.GlobalBrandDbContext
{
    public class GlobalbrandDbContext:IdentityDbContext<User>
    {
        

        public GlobalbrandDbContext(DbContextOptions<GlobalbrandDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Department>()
     .HasMany(d => d.Employees)
     .WithOne(e => e.Department)
     .HasForeignKey(e => e.DeptId)
     .OnDelete(DeleteBehavior.Restrict);




            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Attachment>()
     .HasOne(a => a.Task)
     .WithOne(t => t.Attachments)
     .HasForeignKey<Attachment>(a => a.TaskId)
     .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Task)
                .WithOne(t => t.Comments)
                .HasForeignKey<Comment>(c => c.TaskId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
       .HasOne(e => e.User)
       .WithOne(u => u.Employee)
       .HasForeignKey<User>(u => u.EmployeeId)
       .OnDelete(DeleteBehavior.Cascade);



            modelBuilder.Entity<Department>().HasData(
                new Department { Id = 1, Name = "IT" },
                new Department { Id = 2, Name = "HR" },
                new Department { Id = 3, Name = "Finance" },
                new Department { Id = 4, Name = "Marketing" }
            );

            modelBuilder.Entity<Employee>().HasData(
                new Employee { Id = 1, FirstName = "Ahmed", LastName = "Farag", Salary = 5000, DeptId = 1, ImageURL = "/Images/bohemian-man-with-his-arms-crossed.jpg" },
                new Employee { Id = 2, FirstName = "Mariam", LastName = "Ahmed", Salary = 6000, DeptId = 1, ImageURL = "/Images/causal-female-posing-hat-isolated-white-wall.jpg" },
                new Employee { Id = 3, FirstName = "Abdelrahman", LastName = "Mohammed", Salary = 5500, DeptId = 1, ImageURL = "/Images/smiling-young-man-with-crossed-arms-outdoors.jpg" },
                new Employee { Id = 4, FirstName = "Sara", LastName = "Ali", Salary = 7000, DeptId = 2, ImageURL = "/Images/young-beautiful-woman-pink-warm-sweater-natural-look-smiling-portrait-isolated-long-hair.jpg" },
                new Employee { Id = 5, FirstName = "Aliaa", LastName = "Khaled", Salary = 6500, DeptId = 3, ImageURL = "/Images/causal-female-posing-hat-isolated-white-wall.jpg" },
                new Employee { Id = 6, FirstName = "Hamza", LastName = "Ali", Salary = 8500, DeptId = 4, ImageURL = "/Images/bohemian-man-with-his-arms-crossed.jpg" },
                new Employee { Id = 7, FirstName = "Tarek", LastName = "Salama", Salary = 9500, DeptId = 2, ImageURL = "/Images/smiling-young-man-with-crossed-arms-outdoors.jpg" },
                new Employee { Id = 8, FirstName = "Ali", LastName = "Mohammed", Salary = 12000, DeptId = 3, ImageURL = "/Images/smiling-young-man-with-crossed-arms-outdoors.jpg" },
                new Employee { Id = 9, FirstName = "Mai", LastName = "Alaa", Salary = 15000, DeptId = 4, ImageURL = "/Images/young-woman-posing-outdoor-field.jpg" }
            );

            //modelBuilder.Entity<User>().HasData(
            //    new User { UserId = 1, UserName = "AhmedFarag", Password = "Ahmed2003#", Role = "Manager", EmployeeId = 1 },
            //    new User { UserId = 2, UserName = "MariamAhmed", Password = "Mariam123#", Role = "Employee", EmployeeId = 2 },
            //    new User { UserId = 3, UserName = "AbdelrahmanMohammed", Password = "Abdelrahman123#", Role = "Employee", EmployeeId = 3 },
            //    new User { UserId = 4, UserName = "SaraAli", Password = "Sara123#", Role = "Employee", EmployeeId = 4 },
            //    new User { UserId = 5, UserName = "AliaaAli", Password = "Aliaa123#", Role = "Employee", EmployeeId = 5 },
            //    new User { UserId = 6, UserName = "HamzaAli", Password = "Hamza123#", Role = "Employee", EmployeeId = 6 },
            //    new User { UserId = 7, UserName = "TarekSalama", Password = "Tarek2003#", Role = "Manager", EmployeeId = 7 },
            //    new User { UserId = 8, UserName = "AliMohammed", Password = "Ali2003#", Role = "Manager", EmployeeId = 8 },
            //    new User { UserId = 9, UserName = "MaiAlaa", Password = "Mai2003#", Role = "Manager", EmployeeId = 9 }
            //);

            modelBuilder.Entity<Tasks>().HasData(
                new Tasks { Id = 1, Title = "Prepare monthly IT report", Description = "Compile and analyze IT data.", Status = "Pending", EmployeeId = 2 },
                new Tasks { Id = 2, Title = "IT Policy Update", Description = "Review and update handbook for 2025.", Status = "InProgress", EmployeeId = 3 },
                new Tasks { Id = 3, Title = "System Maintenance", Description = "Perform maintenance on servers.", Status = "Completed", EmployeeId = 4 }
            );
        }


        




        //public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Department> Departments { get; set; } = null!;
        public virtual DbSet<Employee> Employees { get; set; } = null!;
        public virtual DbSet<Tasks> Tasks { get; set; } = null!;
        public virtual DbSet<Comment> Comments { get; set; } = null!;
        public virtual DbSet<Attachment> Attachments { get; set; } = null!;

       
    }
}
