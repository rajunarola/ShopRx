using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class UpdateWatchListTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PharmacyId",
                table: "WatchList",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_WatchList_PharmacyId",
                table: "WatchList",
                column: "PharmacyId");

            migrationBuilder.AddForeignKey(
                name: "FK_WatchList_Pharmacy_PharmacyId",
                table: "WatchList",
                column: "PharmacyId",
                principalTable: "Pharmacy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WatchList_Pharmacy_PharmacyId",
                table: "WatchList");

            migrationBuilder.DropIndex(
                name: "IX_WatchList_PharmacyId",
                table: "WatchList");

            migrationBuilder.DropColumn(
                name: "PharmacyId",
                table: "WatchList");
        }
    }
}
