using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class AddNewPermissionTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Functionality",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Functionality", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccessModuleFunctionality",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RolesModuleId = table.Column<int>(nullable: false),
                    FunctionalityId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessModuleFunctionality", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccessModuleFunctionality_Functionality_FunctionalityId",
                        column: x => x.FunctionalityId,
                        principalTable: "Functionality",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccessModuleFunctionality_RolesModuleAccess_RolesModuleId",
                        column: x => x.RolesModuleId,
                        principalTable: "RolesModuleAccess",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessModuleFunctionality_FunctionalityId",
                table: "AccessModuleFunctionality",
                column: "FunctionalityId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessModuleFunctionality_RolesModuleId",
                table: "AccessModuleFunctionality",
                column: "RolesModuleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessModuleFunctionality");

            migrationBuilder.DropTable(
                name: "Functionality");
        }
    }
}
