using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class UpdateRolesModuleAccess : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RolesModuleAccess_Functionality_FunctionalityId",
                table: "RolesModuleAccess");

            migrationBuilder.AlterColumn<int>(
                name: "FunctionalityId",
                table: "RolesModuleAccess",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_RolesModuleAccess_Functionality_FunctionalityId",
                table: "RolesModuleAccess",
                column: "FunctionalityId",
                principalTable: "Functionality",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RolesModuleAccess_Functionality_FunctionalityId",
                table: "RolesModuleAccess");

            migrationBuilder.AlterColumn<int>(
                name: "FunctionalityId",
                table: "RolesModuleAccess",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RolesModuleAccess_Functionality_FunctionalityId",
                table: "RolesModuleAccess",
                column: "FunctionalityId",
                principalTable: "Functionality",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
