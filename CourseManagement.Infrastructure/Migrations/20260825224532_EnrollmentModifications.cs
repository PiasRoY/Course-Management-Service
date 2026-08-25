using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EnrollmentModifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Users_EnrolledById",
                schema: "course.managment",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_EnrolledById",
                schema: "course.managment",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_StudentId",
                schema: "course.managment",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "EnrolledAt",
                schema: "course.managment",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "EnrolledById",
                schema: "course.managment",
                table: "Enrollments");

            migrationBuilder.AddColumn<Guid>(
                name: "CourseId",
                schema: "course.managment",
                table: "Enrollments",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_CourseId",
                schema: "course.managment",
                table: "Enrollments",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_StudentId_CourseId_ClassId",
                schema: "course.managment",
                table: "Enrollments",
                columns: new[] { "StudentId", "CourseId", "ClassId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Courses_CourseId",
                schema: "course.managment",
                table: "Enrollments",
                column: "CourseId",
                principalSchema: "course.managment",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Courses_CourseId",
                schema: "course.managment",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_CourseId",
                schema: "course.managment",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_StudentId_CourseId_ClassId",
                schema: "course.managment",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "CourseId",
                schema: "course.managment",
                table: "Enrollments");

            migrationBuilder.AddColumn<DateTime>(
                name: "EnrolledAt",
                schema: "course.managment",
                table: "Enrollments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "EnrolledById",
                schema: "course.managment",
                table: "Enrollments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_EnrolledById",
                schema: "course.managment",
                table: "Enrollments",
                column: "EnrolledById");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_StudentId",
                schema: "course.managment",
                table: "Enrollments",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Users_EnrolledById",
                schema: "course.managment",
                table: "Enrollments",
                column: "EnrolledById",
                principalSchema: "course.managment",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
