using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class UpdateDistributorSubscriptionTypeAndHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Brand",
                table: "DistributorSubscriptionHistory",
                type: "decimal(17, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Generic",
                table: "DistributorSubscriptionHistory",
                type: "decimal(17, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Otc",
                table: "DistributorSubscriptionHistory",
                type: "decimal(17, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Brand",
                table: "DistributorSubscription",
                type: "decimal(17, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Generic",
                table: "DistributorSubscription",
                type: "decimal(17, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Otc",
                table: "DistributorSubscription",
                type: "decimal(17, 2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Brand",
                table: "DistributorSubscriptionHistory");

            migrationBuilder.DropColumn(
                name: "Generic",
                table: "DistributorSubscriptionHistory");

            migrationBuilder.DropColumn(
                name: "Otc",
                table: "DistributorSubscriptionHistory");

            migrationBuilder.DropColumn(
                name: "Brand",
                table: "DistributorSubscription");

            migrationBuilder.DropColumn(
                name: "Generic",
                table: "DistributorSubscription");

            migrationBuilder.DropColumn(
                name: "Otc",
                table: "DistributorSubscription");
        }
    }
}
