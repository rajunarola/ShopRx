using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class UpdateOrderTableColumn1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_PharmacyShippingAddress_PharmacyShippingAddressId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_PharmacyShippingAddressId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "PharmacyShippingAddressId",
                table: "Order");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PharmacyShippingAddressId",
                table: "Order",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_PharmacyShippingAddressId",
                table: "Order",
                column: "PharmacyShippingAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_PharmacyShippingAddress_PharmacyShippingAddressId",
                table: "Order",
                column: "PharmacyShippingAddressId",
                principalTable: "PharmacyShippingAddress",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
