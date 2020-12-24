using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class UpdateNewDistributorRequestTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactCity",
                table: "NewDistributorRequest",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ContactStateId",
                table: "NewDistributorRequest",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactZipCode",
                table: "NewDistributorRequest",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NewDistributorRequest_ContactStateId",
                table: "NewDistributorRequest",
                column: "ContactStateId");

            migrationBuilder.AddForeignKey(
                name: "FK_NewDistributorRequest_State_ContactStateId",
                table: "NewDistributorRequest",
                column: "ContactStateId",
                principalTable: "State",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NewDistributorRequest_State_ContactStateId",
                table: "NewDistributorRequest");

            migrationBuilder.DropIndex(
                name: "IX_NewDistributorRequest_ContactStateId",
                table: "NewDistributorRequest");

            migrationBuilder.DropColumn(
                name: "ContactCity",
                table: "NewDistributorRequest");

            migrationBuilder.DropColumn(
                name: "ContactStateId",
                table: "NewDistributorRequest");

            migrationBuilder.DropColumn(
                name: "ContactZipCode",
                table: "NewDistributorRequest");
        }
    }
}
