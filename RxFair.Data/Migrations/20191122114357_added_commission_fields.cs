using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class added_commission_fields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CommissionCountDate",
                table: "DistributorOrderCharge",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionAmount",
                table: "CommissionHistory",
                type: "decimal(17, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "OrderChargeId",
                table: "CommissionHistory",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_CommissionHistory_OrderChargeId",
                table: "CommissionHistory",
                column: "OrderChargeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommissionHistory_DistributorOrderCharge_OrderChargeId",
                table: "CommissionHistory",
                column: "OrderChargeId",
                principalTable: "DistributorOrderCharge",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommissionHistory_DistributorOrderCharge_OrderChargeId",
                table: "CommissionHistory");

            migrationBuilder.DropIndex(
                name: "IX_CommissionHistory_OrderChargeId",
                table: "CommissionHistory");

            migrationBuilder.DropColumn(
                name: "CommissionCountDate",
                table: "DistributorOrderCharge");

            migrationBuilder.DropColumn(
                name: "CommissionAmount",
                table: "CommissionHistory");

            migrationBuilder.DropColumn(
                name: "OrderChargeId",
                table: "CommissionHistory");
        }
    }
}
