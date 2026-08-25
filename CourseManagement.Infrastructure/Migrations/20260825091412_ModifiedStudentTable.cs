using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedStudentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StudentNumber",
                schema: "course.managment",
                table: "Students",
                newName: "RollNumber");

            migrationBuilder.RenameColumn(
                name: "EnrollmentDate",
                schema: "course.managment",
                table: "Students",
                newName: "AdmissionDate");

            migrationBuilder.RenameIndex(
                name: "IX_Students_StudentNumber",
                schema: "course.managment",
                table: "Students",
                newName: "IX_Students_RollNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RollNumber",
                schema: "course.managment",
                table: "Students",
                newName: "StudentNumber");

            migrationBuilder.RenameColumn(
                name: "AdmissionDate",
                schema: "course.managment",
                table: "Students",
                newName: "EnrollmentDate");

            migrationBuilder.RenameIndex(
                name: "IX_Students_RollNumber",
                schema: "course.managment",
                table: "Students",
                newName: "IX_Students_StudentNumber");
        }
    }
}
