using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class UpdateOrderTableColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "BillingAddressId",
                table: "Order");

            migrationBuilder.RenameColumn(
                name: "ShippingAddressId",
                table: "Order",
                newName: "PharmacyShippingAddressId");

            migrationBuilder.RenameIndex(
                name: "IX_Order_ShippingAddressId",
                table: "Order",
                newName: "IX_Order_PharmacyShippingAddressId");

            migrationBuilder.AddColumn<string>(
                name: "BillingAddress",
                table: "Order",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress",
                table: "Order",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_PharmacyShippingAddress_PharmacyShippingAddressId",
                table: "Order",
                column: "PharmacyShippingAddressId",
                principalTable: "PharmacyShippingAddress",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_PharmacyShippingAddress_PharmacyShippingAddressId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "BillingAddress",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ShippingAddress",
                table: "Order");

            migrationBuilder.RenameColumn(
                name: "PharmacyShippingAddressId",
                table: "Order",
                newName: "ShippingAddressId");

            migrationBuilder.RenameIndex(
                name: "IX_Order_PharmacyShippingAddressId",
                table: "Order",
                newName: "IX_Order_ShippingAddressId");

            migrationBuilder.AddColumn<long>(
                name: "BillingAddressId",
                table: "Order",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_BillingAddressId",
                table: "Order",
                column: "BillingAddressId");

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
    }
}
