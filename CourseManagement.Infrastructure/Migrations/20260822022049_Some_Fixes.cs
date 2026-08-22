using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Some_Fixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_UserRole_UserRoles_RoleId",
                schema: "course.managment",
                table: "User_UserRole");

            migrationBuilder.DropForeignKey(
                name: "FK_User_UserRole_Users_UserId",
                schema: "course.managment",
                table: "User_UserRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User_UserRole",
                schema: "course.managment",
                table: "User_UserRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TokenInfosTable",
                schema: "course.managment",
                table: "TokenInfosTable");

            migrationBuilder.RenameTable(
                name: "User_UserRole",
                schema: "course.managment",
                newName: "User_UserRoles",
                newSchema: "course.managment");

            migrationBuilder.RenameTable(
                name: "TokenInfosTable",
                schema: "course.managment",
                newName: "TokenInfos",
                newSchema: "course.managment");

            migrationBuilder.RenameIndex(
                name: "IX_User_UserRole_RoleId",
                schema: "course.managment",
                table: "User_UserRoles",
                newName: "IX_User_UserRoles_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_TokenInfosTable_UserId",
                schema: "course.managment",
                table: "TokenInfos",
                newName: "IX_TokenInfos_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_TokenInfosTable_TokenHash",
                schema: "course.managment",
                table: "TokenInfos",
                newName: "IX_TokenInfos_TokenHash");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User_UserRoles",
                schema: "course.managment",
                table: "User_UserRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TokenInfos",
                schema: "course.managment",
                table: "TokenInfos",
                column: "TokenId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_UserRoles_UserRoles_RoleId",
                schema: "course.managment",
                table: "User_UserRoles",
                column: "RoleId",
                principalSchema: "course.managment",
                principalTable: "UserRoles",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_User_UserRoles_Users_UserId",
                schema: "course.managment",
                table: "User_UserRoles",
                column: "UserId",
                principalSchema: "course.managment",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_UserRoles_UserRoles_RoleId",
                schema: "course.managment",
                table: "User_UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_User_UserRoles_Users_UserId",
                schema: "course.managment",
                table: "User_UserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User_UserRoles",
                schema: "course.managment",
                table: "User_UserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TokenInfos",
                schema: "course.managment",
                table: "TokenInfos");

            migrationBuilder.RenameTable(
                name: "User_UserRoles",
                schema: "course.managment",
                newName: "User_UserRole",
                newSchema: "course.managment");

            migrationBuilder.RenameTable(
                name: "TokenInfos",
                schema: "course.managment",
                newName: "TokenInfosTable",
                newSchema: "course.managment");

            migrationBuilder.RenameIndex(
                name: "IX_User_UserRoles_RoleId",
                schema: "course.managment",
                table: "User_UserRole",
                newName: "IX_User_UserRole_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_TokenInfos_UserId",
                schema: "course.managment",
                table: "TokenInfosTable",
                newName: "IX_TokenInfosTable_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_TokenInfos_TokenHash",
                schema: "course.managment",
                table: "TokenInfosTable",
                newName: "IX_TokenInfosTable_TokenHash");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User_UserRole",
                schema: "course.managment",
                table: "User_UserRole",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TokenInfosTable",
                schema: "course.managment",
                table: "TokenInfosTable",
                column: "TokenId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_UserRole_UserRoles_RoleId",
                schema: "course.managment",
                table: "User_UserRole",
                column: "RoleId",
                principalSchema: "course.managment",
                principalTable: "UserRoles",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_User_UserRole_Users_UserId",
                schema: "course.managment",
                table: "User_UserRole",
                column: "UserId",
                principalSchema: "course.managment",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
