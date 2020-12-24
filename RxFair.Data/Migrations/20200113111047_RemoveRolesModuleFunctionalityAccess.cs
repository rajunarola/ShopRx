using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class RemoveRolesModuleFunctionalityAccess : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RolesModuleFunctionalityAccess");

            migrationBuilder.AddColumn<int>(
                name: "FunctionalityId",
                table: "RolesModuleAccess",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RolesModuleAccess_FunctionalityId",
                table: "RolesModuleAccess",
                column: "FunctionalityId");

            migrationBuilder.AddForeignKey(
                name: "FK_RolesModuleAccess_Functionality_FunctionalityId",
                table: "RolesModuleAccess",
                column: "FunctionalityId",
                principalTable: "Functionality",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RolesModuleAccess_Functionality_FunctionalityId",
                table: "RolesModuleAccess");

            migrationBuilder.DropIndex(
                name: "IX_RolesModuleAccess_FunctionalityId",
                table: "RolesModuleAccess");

            migrationBuilder.DropColumn(
                name: "FunctionalityId",
                table: "RolesModuleAccess");

            migrationBuilder.CreateTable(
                name: "RolesModuleFunctionalityAccess",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FunctionalityId = table.Column<int>(nullable: false),
                    RolesModuleId = table.Column<int>(nullable: false)
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
        }
    }
}
