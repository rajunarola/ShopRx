using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class AddSystemModuleTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SystemModule",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ModuleName = table.Column<string>(nullable: true),
                    ParentsId = table.Column<int>(nullable: true),
                    MenuDisplayText = table.Column<string>(nullable: true),
                    Controller = table.Column<string>(nullable: true),
                    Action = table.Column<string>(nullable: true),
                    IsField = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemModule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemModule_SystemModule_ParentsId",
                        column: x => x.ParentsId,
                        principalTable: "SystemModule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RolesModuleAccess",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<long>(nullable: false),
                    ModuleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolesModuleAccess", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolesModuleAccess_SystemModule_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "SystemModule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolesModuleAccess_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RolesModuleAccess_ModuleId",
                table: "RolesModuleAccess",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_RolesModuleAccess_RoleId",
                table: "RolesModuleAccess",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemModule_ParentsId",
                table: "SystemModule",
                column: "ParentsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RolesModuleAccess");

            migrationBuilder.DropTable(
                name: "SystemModule");
        }
    }
}
