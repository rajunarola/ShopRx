using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class UpdateDataTypeLength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Redeem",
                table: "RewardProduct",
                type: "decimal(17, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5, 2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Referral",
                table: "RewardMoneyMaster",
                type: "decimal(17, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5, 2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "RewardMoney",
                table: "RewardEarn",
                type: "decimal(17, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5, 2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ShippingCharge",
                table: "DistributorOrderSetting",
                type: "decimal(17, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5, 2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "OverNightAmount",
                table: "DistributorOrderSetting",
                type: "decimal(17, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5, 2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinOrderAmount",
                table: "DistributorOrderSetting",
                type: "decimal(17, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5, 2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ChargedMonthly",
                table: "CommissionHistory",
                type: "decimal(17, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5, 2)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Redeem",
                table: "RewardProduct",
                type: "decimal(5, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(17, 2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Referral",
                table: "RewardMoneyMaster",
                type: "decimal(5, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(17, 2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "RewardMoney",
                table: "RewardEarn",
                type: "decimal(5, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(17, 2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ShippingCharge",
                table: "DistributorOrderSetting",
                type: "decimal(5, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(17, 2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "OverNightAmount",
                table: "DistributorOrderSetting",
                type: "decimal(5, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(17, 2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinOrderAmount",
                table: "DistributorOrderSetting",
                type: "decimal(5, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(17, 2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ChargedMonthly",
                table: "CommissionHistory",
                type: "decimal(5, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(17, 2)");
        }
    }
}
