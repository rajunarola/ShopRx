using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class UpdateOrderTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UniqueOrder",
                table: "Order",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "OrderStatus",
                table: "DistributorOrderCharge",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<decimal>(
                name: "PaymentAmount",
                table: "DistributorOrderCharge",
                type: "decimal(17, 2)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "DistributorOrderCharge",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentNote",
                table: "DistributorOrderCharge",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ShippingDate",
                table: "DistributorOrderCharge",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrackingLink",
                table: "DistributorOrderCharge",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrackingNo",
                table: "DistributorOrderCharge",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UniqueOrder",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "OrderStatus",
                table: "DistributorOrderCharge");

            migrationBuilder.DropColumn(
                name: "PaymentAmount",
                table: "DistributorOrderCharge");

            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "DistributorOrderCharge");

            migrationBuilder.DropColumn(
                name: "PaymentNote",
                table: "DistributorOrderCharge");

            migrationBuilder.DropColumn(
                name: "ShippingDate",
                table: "DistributorOrderCharge");

            migrationBuilder.DropColumn(
                name: "TrackingLink",
                table: "DistributorOrderCharge");

            migrationBuilder.DropColumn(
                name: "TrackingNo",
                table: "DistributorOrderCharge");
        }
    }
}
