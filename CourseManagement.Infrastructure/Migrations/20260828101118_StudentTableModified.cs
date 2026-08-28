using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class StudentTableModified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CGPA",
                schema: "course.managment",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "TotalCreditsTaken",
                schema: "course.managment",
                table: "Students");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CGPA",
                schema: "course.managment",
                table: "Students",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TotalCreditsTaken",
                schema: "course.managment",
                table: "Students",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
