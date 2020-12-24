using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class UpdateRewardTypeMasterTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RewardType",
                table: "RewardMoneyMaster");

            migrationBuilder.AddColumn<long>(
                name: "RewardTypeId",
                table: "RewardMoneyMaster",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_RewardMoneyMaster_RewardTypeId",
                table: "RewardMoneyMaster",
                column: "RewardTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_RewardMoneyMaster_RewardTypeMaster_RewardTypeId",
                table: "RewardMoneyMaster",
                column: "RewardTypeId",
                principalTable: "RewardTypeMaster",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RewardMoneyMaster_RewardTypeMaster_RewardTypeId",
                table: "RewardMoneyMaster");

            migrationBuilder.DropIndex(
                name: "IX_RewardMoneyMaster_RewardTypeId",
                table: "RewardMoneyMaster");

            migrationBuilder.DropColumn(
                name: "RewardTypeId",
                table: "RewardMoneyMaster");

            migrationBuilder.AddColumn<int>(
                name: "RewardType",
                table: "RewardMoneyMaster",
                nullable: false,
                defaultValue: 0);
        }
    }
}
