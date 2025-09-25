using System.Data;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Data.Models.Common;
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
               

            modelBuilder.Entity<Employee>()
     .HasOne(e => e.Manager)
     .WithMany(m => m.Subordinates)
     .HasForeignKey(e => e.ManagerId)
     .OnDelete(DeleteBehavior.Restrict); 



            modelBuilder.Entity<Employee>()
                .Property(e => e.Roles).HasConversion<string>();

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


          
            modelBuilder.Entity<Department>()
                .HasMany(d => d.Employees)
                .WithOne(e => e.Department)
                .HasForeignKey(e => e.DeptId)
                .OnDelete(DeleteBehavior.Restrict);

          
            modelBuilder.Entity<Department>()
                .HasOne(d => d.Manager)
                .WithMany() 
                .HasForeignKey(d => d.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);



            modelBuilder.Entity<Department>().HasData(
       new Department { Id = 1, Name = "IT"},
       new Department { Id = 2, Name = "HR"},
       new Department { Id = 3, Name = "Finance"},
       new Department { Id = 4, Name = "Marketing" }
   );

            modelBuilder.Entity<Employee>().HasData(
    new Employee { Id = 1, FirstName = "Ahmed", LastName = "Farag", Password = "Ahmed2003#", Salary = 5000, DeptId = 1, Roles = Role.Manager, ManagerId = null, ImageURL = "/Images/bohemian-man-with-his-arms-crossed.jpg" },
    new Employee { Id = 2, FirstName = "Mariam", LastName = "Ahmed", Password = "Mariam123#", Salary = 6000, DeptId = 1, Roles = Role.Employee, ImageURL = "/Images/causal-female-posing-hat-isolated-white-wall.jpg" },
    new Employee { Id = 3, FirstName = "Abdelrahman", LastName = "Mohammed", Password = "Abdelrahman123#", Salary = 5500, DeptId = 1, Roles = Role.Employee, ImageURL = "/Images/smiling-young-man-with-crossed-arms-outdoors.jpg" },
    new Employee { Id = 4, FirstName = "Sara", LastName = "Ali", Password = "Sara123#", Salary = 7000, DeptId = 2, Roles = Role.Employee, ImageURL = "/Images/young-beautiful-woman.jpg" },
    new Employee { Id = 5, FirstName = "Aliaa", LastName = "Khaled", Password = "Aliaa123#", Salary = 6500, DeptId = 3, Roles = Role.Employee, ImageURL = "/Images/causal-female-posing-hat-isolated-white-wall.jpg" },
    new Employee { Id = 6, FirstName = "Hamza", LastName = "Ali", Password = "Hamza123#", Salary = 8500, DeptId = 4, Roles = Role.Employee, ImageURL = "/Images/bohemian-man-with-his-arms-crossed.jpg" },
    new Employee { Id = 7, FirstName = "Tarek", LastName = "Salama", Password = "Tarek2003#", Salary = 9500, DeptId = 2, Roles = Role.Manager, ImageURL = "/Images/smiling-young-man-with-crossed-arms-outdoors.jpg" },
    new Employee { Id = 8, FirstName = "Ali", LastName = "Mohammed", Password = "Ali2003#", Salary = 12000, DeptId = 3, Roles = Role.Manager, ImageURL = "/Images/smiling-young-man-with-crossed-arms-outdoors.jpg" },
    new Employee { Id = 9, FirstName = "Mai", LastName = "Alaa", Password = "Mai2003#", Salary = 15000, DeptId = 4, Roles = Role.Manager, ImageURL = "/Images/young-woman-posing-outdoor-field.jpg" }
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

            base.OnModelCreating(modelBuilder);
        }


        




        //public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Department> Departments { get; set; } = null!;
        public virtual DbSet<Employee> Employees { get; set; } = null!;
        public virtual DbSet<Tasks> Tasks { get; set; } = null!;
        public virtual DbSet<Comment> Comments { get; set; } = null!;
        public virtual DbSet<Attachment> Attachments { get; set; } = null!;

       
    }
}
