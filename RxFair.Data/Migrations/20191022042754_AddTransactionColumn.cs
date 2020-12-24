using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class AddTransactionColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPayment",
                table: "DistributorSubscriptionHistory",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PayPalTransactionId",
                table: "DistributorSubscriptionHistory",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "DistributorSubscriptionHistory",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PayPalTransactionId",
                table: "DistributorSubscription",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPayment",
                table: "DistributorSubscriptionHistory");

            migrationBuilder.DropColumn(
                name: "PayPalTransactionId",
                table: "DistributorSubscriptionHistory");

            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "DistributorSubscriptionHistory");

            migrationBuilder.DropColumn(
                name: "PayPalTransactionId",
                table: "DistributorSubscription");
        }
    }
}
