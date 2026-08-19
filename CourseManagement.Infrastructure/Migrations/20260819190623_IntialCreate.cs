using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IntialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "course.managment");

            migrationBuilder.CreateTable(
                name: "Courses",
                schema: "course.managment",
                columns: table => new
                {
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Title = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Credits = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.CourseId);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                schema: "course.managment",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "course.managment",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmailAddress = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", maxLength: 2147483647, nullable: false),
                    FirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Classes",
                schema: "course.managment",
                columns: table => new
                {
                    ClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Semester = table.Column<string>(type: "text", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    SectionCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    InstructorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classes", x => x.ClassId);
                    table.ForeignKey(
                        name: "FK_Classes_Users_InstructorId",
                        column: x => x.InstructorId,
                        principalSchema: "course.managment",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "User_UserRole",
                schema: "course.managment",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_UserRole", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_User_UserRole_UserRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "course.managment",
                        principalTable: "UserRoles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_User_UserRole_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "course.managment",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CoursesClasses",
                schema: "course.managment",
                columns: table => new
                {
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoursesClasses", x => new { x.CourseId, x.ClassId });
                    table.ForeignKey(
                        name: "FK_CoursesClasses_Classes_ClassId",
                        column: x => x.ClassId,
                        principalSchema: "course.managment",
                        principalTable: "Classes",
                        principalColumn: "ClassId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CoursesClasses_Courses_CourseId",
                        column: x => x.CourseId,
                        principalSchema: "course.managment",
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Enrollments",
                schema: "course.managment",
                columns: table => new
                {
                    EnrollmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    EnrolledById = table.Column<Guid>(type: "uuid", nullable: false),
                    EnrolledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrollments", x => x.EnrollmentId);
                    table.ForeignKey(
                        name: "FK_Enrollments_Classes_ClassId",
                        column: x => x.ClassId,
                        principalSchema: "course.managment",
                        principalTable: "Classes",
                        principalColumn: "ClassId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Enrollments_Users_EnrolledById",
                        column: x => x.EnrolledById,
                        principalSchema: "course.managment",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Enrollments_Users_StudentId",
                        column: x => x.StudentId,
                        principalSchema: "course.managment",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Classes_InstructorId",
                schema: "course.managment",
                table: "Classes",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_Name",
                schema: "course.managment",
                table: "Classes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_Name",
                schema: "course.managment",
                table: "Courses",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CoursesClasses_ClassId",
                schema: "course.managment",
                table: "CoursesClasses",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_ClassId",
                schema: "course.managment",
                table: "Enrollments",
                column: "ClassId");

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

            migrationBuilder.CreateIndex(
                name: "IX_User_UserRole_RoleId",
                schema: "course.managment",
                table: "User_UserRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_EmailAddress",
                schema: "course.managment",
                table: "Users",
                column: "EmailAddress",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoursesClasses",
                schema: "course.managment");

            migrationBuilder.DropTable(
                name: "Enrollments",
                schema: "course.managment");

            migrationBuilder.DropTable(
                name: "User_UserRole",
                schema: "course.managment");

            migrationBuilder.DropTable(
                name: "Courses",
                schema: "course.managment");

            migrationBuilder.DropTable(
                name: "Classes",
                schema: "course.managment");

            migrationBuilder.DropTable(
                name: "UserRoles",
                schema: "course.managment");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "course.managment");
        }
    }
}
