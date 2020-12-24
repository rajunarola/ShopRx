using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class UdpateDistributorTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactCity",
                table: "Distributor",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ContactStateId",
                table: "Distributor",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactZipCode",
                table: "Distributor",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Distributor_ContactStateId",
                table: "Distributor",
                column: "ContactStateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Distributor_State_ContactStateId",
                table: "Distributor",
                column: "ContactStateId",
                principalTable: "State",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Distributor_State_ContactStateId",
                table: "Distributor");

            migrationBuilder.DropIndex(
                name: "IX_Distributor_ContactStateId",
                table: "Distributor");

            migrationBuilder.DropColumn(
                name: "ContactCity",
                table: "Distributor");

            migrationBuilder.DropColumn(
                name: "ContactStateId",
                table: "Distributor");

            migrationBuilder.DropColumn(
                name: "ContactZipCode",
                table: "Distributor");
        }
    }
}
