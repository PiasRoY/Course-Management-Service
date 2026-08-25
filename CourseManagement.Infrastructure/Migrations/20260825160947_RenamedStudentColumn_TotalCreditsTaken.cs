using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenamedStudentColumn_TotalCreditsTaken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalCreditsEarned",
                schema: "course.managment",
                table: "Students",
                newName: "TotalCreditsTaken");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalCreditsTaken",
                schema: "course.managment",
                table: "Students",
                newName: "TotalCreditsEarned");
        }
    }
}
