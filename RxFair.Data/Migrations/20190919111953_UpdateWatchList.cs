using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class UpdateWatchList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DistributorId",
                table: "WatchList",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_WatchList_DistributorId",
                table: "WatchList",
                column: "DistributorId");

            migrationBuilder.AddForeignKey(
                name: "FK_WatchList_Distributor_DistributorId",
                table: "WatchList",
                column: "DistributorId",
                principalTable: "Distributor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WatchList_Distributor_DistributorId",
                table: "WatchList");

            migrationBuilder.DropIndex(
                name: "IX_WatchList_DistributorId",
                table: "WatchList");

            migrationBuilder.DropColumn(
                name: "DistributorId",
                table: "WatchList");
        }
    }
}
