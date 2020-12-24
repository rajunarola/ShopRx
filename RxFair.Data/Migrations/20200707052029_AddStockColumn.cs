using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class AddStockColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Stock",
                table: "MedicinePriceMasters",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Stock",
                table: "MedicinePriceHistory",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stock",
                table: "MedicinePriceMasters");

            migrationBuilder.DropColumn(
                name: "Stock",
                table: "MedicinePriceHistory");
        }
    }
}
