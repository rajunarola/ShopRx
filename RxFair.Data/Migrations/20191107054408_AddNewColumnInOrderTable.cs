using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class AddNewColumnInOrderTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BillingAddressId",
                table: "Order",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ShippingAddressId",
                table: "Order",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_BillingAddressId",
                table: "Order",
                column: "BillingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_ShippingAddressId",
                table: "Order",
                column: "ShippingAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_PharmacyBillingAddress_BillingAddressId",
                table: "Order",
                column: "BillingAddressId",
                principalTable: "PharmacyBillingAddress",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_PharmacyShippingAddress_ShippingAddressId",
                table: "Order",
                column: "ShippingAddressId",
                principalTable: "PharmacyShippingAddress",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_PharmacyBillingAddress_BillingAddressId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_PharmacyShippingAddress_ShippingAddressId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_BillingAddressId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_ShippingAddressId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "BillingAddressId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ShippingAddressId",
                table: "Order");
        }
    }
}
