using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class UpdateSubscriptionType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Brand",
                table: "SubscriptionType",
                type: "decimal(17, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Generic",
                table: "SubscriptionType",
                type: "decimal(17, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Otc",
                table: "SubscriptionType",
                type: "decimal(17, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "RewardPoints",
                table: "OrderWiseRewards",
                type: "decimal(17, 2)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "Reward",
                table: "OrderWiseRewards",
                type: "decimal(17, 2)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "CommissionRate",
                table: "CategoryCommissions",
                type: "decimal(17, 2)",
                nullable: false,
                oldClrType: typeof(decimal));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Brand",
                table: "SubscriptionType");

            migrationBuilder.DropColumn(
                name: "Generic",
                table: "SubscriptionType");

            migrationBuilder.DropColumn(
                name: "Otc",
                table: "SubscriptionType");

            migrationBuilder.AlterColumn<decimal>(
                name: "RewardPoints",
                table: "OrderWiseRewards",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(17, 2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Reward",
                table: "OrderWiseRewards",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(17, 2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CommissionRate",
                table: "CategoryCommissions",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(17, 2)");
        }
    }
}
