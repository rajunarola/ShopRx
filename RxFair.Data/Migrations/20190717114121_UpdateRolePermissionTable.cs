using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class UpdateRolePermissionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ManageAccessPagePermission_AccessPage_AccessPageId",
                table: "ManageAccessPagePermission");

            migrationBuilder.DropIndex(
                name: "IX_ManageAccessPagePermission_AccessPageId",
                table: "ManageAccessPagePermission");

            migrationBuilder.DropColumn(
                name: "AccessPageId",
                table: "ManageAccessPagePermission");

            migrationBuilder.AddColumn<long>(
                name: "AccessPageId",
                table: "PagePermission",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "PermissionId",
                table: "PagePermission",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "Permission",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<long>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<long>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PagePermission_AccessPageId",
                table: "PagePermission",
                column: "AccessPageId");

            migrationBuilder.CreateIndex(
                name: "IX_PagePermission_PermissionId",
                table: "PagePermission",
                column: "PermissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_PagePermission_AccessPage_AccessPageId",
                table: "PagePermission",
                column: "AccessPageId",
                principalTable: "AccessPage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PagePermission_Permission_PermissionId",
                table: "PagePermission",
                column: "PermissionId",
                principalTable: "Permission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PagePermission_AccessPage_AccessPageId",
                table: "PagePermission");

            migrationBuilder.DropForeignKey(
                name: "FK_PagePermission_Permission_PermissionId",
                table: "PagePermission");

            migrationBuilder.DropTable(
                name: "Permission");

            migrationBuilder.DropIndex(
                name: "IX_PagePermission_AccessPageId",
                table: "PagePermission");

            migrationBuilder.DropIndex(
                name: "IX_PagePermission_PermissionId",
                table: "PagePermission");

            migrationBuilder.DropColumn(
                name: "AccessPageId",
                table: "PagePermission");

            migrationBuilder.DropColumn(
                name: "PermissionId",
                table: "PagePermission");

            migrationBuilder.AddColumn<long>(
                name: "AccessPageId",
                table: "ManageAccessPagePermission",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_ManageAccessPagePermission_AccessPageId",
                table: "ManageAccessPagePermission",
                column: "AccessPageId");

            migrationBuilder.AddForeignKey(
                name: "FK_ManageAccessPagePermission_AccessPage_AccessPageId",
                table: "ManageAccessPagePermission",
                column: "AccessPageId",
                principalTable: "AccessPage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
