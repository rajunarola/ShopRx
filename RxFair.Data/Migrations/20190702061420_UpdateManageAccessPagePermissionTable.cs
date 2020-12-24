using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class UpdateManageAccessPagePermissionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "RoleId",
                table: "ManageAccessPagePermission",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 450);

            migrationBuilder.CreateIndex(
                name: "IX_ManageAccessPagePermission_RoleId",
                table: "ManageAccessPagePermission",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_ManageAccessPagePermission_AspNetRoles_RoleId",
                table: "ManageAccessPagePermission",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ManageAccessPagePermission_AspNetRoles_RoleId",
                table: "ManageAccessPagePermission");

            migrationBuilder.DropIndex(
                name: "IX_ManageAccessPagePermission_RoleId",
                table: "ManageAccessPagePermission");

            migrationBuilder.AlterColumn<string>(
                name: "RoleId",
                table: "ManageAccessPagePermission",
                maxLength: 450,
                nullable: false,
                oldClrType: typeof(long));
        }
    }
}
