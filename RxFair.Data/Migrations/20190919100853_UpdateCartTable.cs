using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class UpdateCartTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PharmacyId",
                table: "Cart",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Cart_PharmacyId",
                table: "Cart",
                column: "PharmacyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_Pharmacy_PharmacyId",
                table: "Cart",
                column: "PharmacyId",
                principalTable: "Pharmacy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cart_Pharmacy_PharmacyId",
                table: "Cart");

            migrationBuilder.DropIndex(
                name: "IX_Cart_PharmacyId",
                table: "Cart");

            migrationBuilder.DropColumn(
                name: "PharmacyId",
                table: "Cart");
        }
    }
}
