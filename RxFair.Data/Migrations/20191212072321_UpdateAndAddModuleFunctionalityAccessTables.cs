using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class UpdateAndAddModuleFunctionalityAccessTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccessModuleFunctionality_RolesModuleAccess_RolesModuleId",
                table: "AccessModuleFunctionality");

            migrationBuilder.RenameColumn(
                name: "RolesModuleId",
                table: "AccessModuleFunctionality",
                newName: "ModuleId");

            migrationBuilder.RenameIndex(
                name: "IX_AccessModuleFunctionality_RolesModuleId",
                table: "AccessModuleFunctionality",
                newName: "IX_AccessModuleFunctionality_ModuleId");

            migrationBuilder.CreateTable(
                name: "RolesModuleFunctionalityAccess",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RolesModuleId = table.Column<int>(nullable: false),
                    FunctionalityId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolesModuleFunctionalityAccess", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolesModuleFunctionalityAccess_Functionality_FunctionalityId",
                        column: x => x.FunctionalityId,
                        principalTable: "Functionality",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolesModuleFunctionalityAccess_RolesModuleAccess_RolesModuleId",
                        column: x => x.RolesModuleId,
                        principalTable: "RolesModuleAccess",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RolesModuleFunctionalityAccess_FunctionalityId",
                table: "RolesModuleFunctionalityAccess",
                column: "FunctionalityId");

            migrationBuilder.CreateIndex(
                name: "IX_RolesModuleFunctionalityAccess_RolesModuleId",
                table: "RolesModuleFunctionalityAccess",
                column: "RolesModuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccessModuleFunctionality_SystemModule_ModuleId",
                table: "AccessModuleFunctionality",
                column: "ModuleId",
                principalTable: "SystemModule",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccessModuleFunctionality_SystemModule_ModuleId",
                table: "AccessModuleFunctionality");

            migrationBuilder.DropTable(
                name: "RolesModuleFunctionalityAccess");

            migrationBuilder.RenameColumn(
                name: "ModuleId",
                table: "AccessModuleFunctionality",
                newName: "RolesModuleId");

            migrationBuilder.RenameIndex(
                name: "IX_AccessModuleFunctionality_ModuleId",
                table: "AccessModuleFunctionality",
                newName: "IX_AccessModuleFunctionality_RolesModuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccessModuleFunctionality_RolesModuleAccess_RolesModuleId",
                table: "AccessModuleFunctionality",
                column: "RolesModuleId",
                principalTable: "RolesModuleAccess",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
