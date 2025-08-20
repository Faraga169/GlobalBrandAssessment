using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GlobalBrandAssessment.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ManagerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Salary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImageURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManagerId = table.Column<int>(type: "int", nullable: true),
                    DeptId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_Departments_DeptId",
                        column: x => x.DeptId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employees_Employees_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "ManagerId", "Name" },
                values: new object[,]
                {
                    { 1, null, "IT" },
                    { 2, null, "HR" },
                    { 3, null, "Finance" },
                    { 4, null, "Marketing" }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "DeptId", "FirstName", "ImageURL", "LastName", "ManagerId", "Salary" },
                values: new object[,]
                {
                    { 1, 1, "Ahmed", "/Images/bohemian-man-with-his-arms-crossed.jpg", "Farag", null, 5000m },
                    { 2, 1, "Mariam", "/Images/causal-female-posing-hat-isolated-white-wall.jpg", "Ahmed", null, 6000m },
                    { 3, 1, "Abdelrahman", "/Images/smiling-young-man-with-crossed-arms-outdoors.jpg", "Mohammed", null, 5500m },
                    { 4, 2, "Sara", "/Images/young-beautiful-woman-pink-warm-sweater-natural-look-smiling-portrait-isolated-long-hair.jpg", "Ali", null, 7000m },
                    { 5, 3, "Aliaa", "/Images/causal-female-posing-hat-isolated-white-wall.jpg", "Khaled", null, 6500m },
                    { 6, 4, "Hamza", "/Images/bohemian-man-with-his-arms-crossed.jpg", "Ali", null, 8500m },
                    { 7, 2, "Tarek", "/Images/smiling-young-man-with-crossed-arms-outdoors.jpg", "Salama", null, 9500m },
                    { 8, 3, "Ali", "/Images/smiling-young-man-with-crossed-arms-outdoors.jpg", "Mohammed", null, 12000m },
                    { 9, 4, "Mai", "/Images/young-woman-posing-outdoor-field.jpg", "Alaa", null, 15000m }
                });

            migrationBuilder.InsertData(
                table: "Tasks",
                columns: new[] { "Id", "Description", "EmployeeId", "Status", "Title" },
                values: new object[,]
                {
                    { 1, "Compile and analyze IT data.", 2, "Pending", "Prepare monthly IT report" },
                    { 2, "Review and update handbook for 2025.", 3, "InProgress", "IT Policy Update" },
                    { 3, "Perform maintenance on servers.", 4, "Completed", "System Maintenance" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "EmployeeId", "Password", "Role", "UserName" },
                values: new object[,]
                {
                    { 1, 1, "Ahmed2003#", "Manager", "AhmedFarag" },
                    { 2, 2, "Mariam123#", "Employee", "MariamAhmed" },
                    { 3, 3, "Abdelrahman123#", "Employee", "AbdelrahmanMohammed" },
                    { 4, 4, "Sara123#", "Employee", "SaraAli" },
                    { 5, 5, "Aliaa123#", "Employee", "AliaaAli" },
                    { 6, 6, "Hamza123#", "Employee", "HamzaAli" },
                    { 7, 7, "Tarek2003#", "Manager", "TarekSalama" },
                    { 8, 8, "Ali2003#", "Manager", "AliMohammed" },
                    { 9, 9, "Mai2003#", "Manager", "MaiAlaa" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Departments_ManagerId",
                table: "Departments",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DeptId",
                table: "Employees",
                column: "DeptId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ManagerId",
                table: "Employees",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_EmployeeId",
                table: "Tasks",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_EmployeeId",
                table: "Users",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Employees_ManagerId",
                table: "Departments",
                column: "ManagerId",
                principalTable: "Employees",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Employees_ManagerId",
                table: "Departments");

            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Departments");
        }
    }
}
