using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GlobalBrandAssessment.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddmanagerToDepartment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Employees
            migrationBuilder.UpdateData("Employees", "Id", 2, "ManagerId", 1);
            migrationBuilder.UpdateData("Employees", "Id", 3, "ManagerId", 1);
            migrationBuilder.UpdateData("Employees", "Id", 4, "ManagerId", 7);
            migrationBuilder.UpdateData("Employees", "Id", 5, "ManagerId", 8);
            migrationBuilder.UpdateData("Employees", "Id", 6, "ManagerId", 9);

            // Departments
            migrationBuilder.UpdateData("Departments", "Id", 1, "ManagerId", 1);
            migrationBuilder.UpdateData("Departments", "Id", 2, "ManagerId", 7);
            migrationBuilder.UpdateData("Departments", "Id", 3, "ManagerId", 8);
            migrationBuilder.UpdateData("Departments", "Id", 4, "ManagerId", 9);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
