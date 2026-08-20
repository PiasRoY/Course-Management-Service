using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Student_Table_Created : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Users_StudentId",
                schema: "course.managment",
                table: "Enrollments");

            migrationBuilder.CreateTable(
                name: "Students",
                schema: "course.managment",
                columns: table => new
                {
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    EnrollmentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GraduationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CGPA = table.Column<double>(type: "double precision", nullable: true),
                    TotalCreditsEarned = table.Column<double>(type: "double precision", nullable: false),
                    CurrentTerm = table.Column<int>(type: "integer", nullable: false),
                    CurrentSemester = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.StudentId);
                    table.ForeignKey(
                        name: "FK_Students_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "course.managment",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Students_StudentNumber",
                schema: "course.managment",
                table: "Students",
                column: "StudentNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_UserId",
                schema: "course.managment",
                table: "Students",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Students_StudentId",
                schema: "course.managment",
                table: "Enrollments",
                column: "StudentId",
                principalSchema: "course.managment",
                principalTable: "Students",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Students_StudentId",
                schema: "course.managment",
                table: "Enrollments");

            migrationBuilder.DropTable(
                name: "Students",
                schema: "course.managment");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Users_StudentId",
                schema: "course.managment",
                table: "Enrollments",
                column: "StudentId",
                principalSchema: "course.managment",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
