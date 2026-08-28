using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CheckConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "Check_Courses_Name_Alphanumeric",
                schema: "course.managment",
                table: "Courses",
                sql: "\"Name\" ~ '^[a-zA-Z0-9]+$'");

            migrationBuilder.AddCheckConstraint(
                name: "Check_Classes_Name_Alphanumeric",
                schema: "course.managment",
                table: "Classes",
                sql: "\"Name\" ~ '^[a-zA-Z0-9]+$'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "Check_Courses_Name_Alphanumeric",
                schema: "course.managment",
                table: "Courses");

            migrationBuilder.DropCheckConstraint(
                name: "Check_Classes_Name_Alphanumeric",
                schema: "course.managment",
                table: "Classes");
        }
    }
}
