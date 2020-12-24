using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class UpdateSubscriptionTypeHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Brand",
                table: "SubscriptionTypeHistory",
                type: "decimal(17, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Generic",
                table: "SubscriptionTypeHistory",
                type: "decimal(17, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Otc",
                table: "SubscriptionTypeHistory",
                type: "decimal(17, 2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Brand",
                table: "SubscriptionTypeHistory");

            migrationBuilder.DropColumn(
                name: "Generic",
                table: "SubscriptionTypeHistory");

            migrationBuilder.DropColumn(
                name: "Otc",
                table: "SubscriptionTypeHistory");
        }
    }
}
