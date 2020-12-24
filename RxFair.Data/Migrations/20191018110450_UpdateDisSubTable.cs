using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class UpdateDisSubTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "ChargedMonthly",
                table: "SubscriptionType",
                type: "decimal(17, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5, 2)");

            migrationBuilder.AddColumn<decimal>(
                name: "SubscriptionCharge",
                table: "SubscriptionType",
                type: "decimal(17, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ChargedMonthly",
                table: "DistributorSubscriptionHistory",
                type: "decimal(17, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "DistributorSubscriptionId",
                table: "DistributorSubscriptionHistory",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SubscriptionCharge",
                table: "DistributorSubscriptionHistory",
                type: "decimal(17, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ChargedMonthly",
                table: "DistributorSubscription",
                type: "decimal(17, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsPayment",
                table: "DistributorSubscription",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsUpgraded",
                table: "DistributorSubscription",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "DistributorSubscription",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SubscriptionCharge",
                table: "DistributorSubscription",
                type: "decimal(17, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_DistributorSubscriptionHistory_DistributorSubscriptionId",
                table: "DistributorSubscriptionHistory",
                column: "DistributorSubscriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_DistributorSubscriptionHistory_DistributorSubscription_DistributorSubscriptionId",
                table: "DistributorSubscriptionHistory",
                column: "DistributorSubscriptionId",
                principalTable: "DistributorSubscription",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DistributorSubscriptionHistory_DistributorSubscription_DistributorSubscriptionId",
                table: "DistributorSubscriptionHistory");

            migrationBuilder.DropIndex(
                name: "IX_DistributorSubscriptionHistory_DistributorSubscriptionId",
                table: "DistributorSubscriptionHistory");

            migrationBuilder.DropColumn(
                name: "SubscriptionCharge",
                table: "SubscriptionType");

            migrationBuilder.DropColumn(
                name: "ChargedMonthly",
                table: "DistributorSubscriptionHistory");

            migrationBuilder.DropColumn(
                name: "DistributorSubscriptionId",
                table: "DistributorSubscriptionHistory");

            migrationBuilder.DropColumn(
                name: "SubscriptionCharge",
                table: "DistributorSubscriptionHistory");

            migrationBuilder.DropColumn(
                name: "ChargedMonthly",
                table: "DistributorSubscription");

            migrationBuilder.DropColumn(
                name: "IsPayment",
                table: "DistributorSubscription");

            migrationBuilder.DropColumn(
                name: "IsUpgraded",
                table: "DistributorSubscription");

            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "DistributorSubscription");

            migrationBuilder.DropColumn(
                name: "SubscriptionCharge",
                table: "DistributorSubscription");

            migrationBuilder.AlterColumn<decimal>(
                name: "ChargedMonthly",
                table: "SubscriptionType",
                type: "decimal(5, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(17, 2)");
        }
    }
}
