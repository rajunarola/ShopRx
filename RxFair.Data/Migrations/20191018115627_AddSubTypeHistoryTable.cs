using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class AddSubTypeHistoryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommissionHistory_SubscriptionType_SubscriptionTypeId",
                table: "CommissionHistory");

            migrationBuilder.DropIndex(
                name: "IX_CommissionHistory_SubscriptionTypeId",
                table: "CommissionHistory");

            migrationBuilder.DropColumn(
                name: "ChargedMonthly",
                table: "CommissionHistory");

            migrationBuilder.DropColumn(
                name: "SubscriptionTypeId",
                table: "CommissionHistory");

            migrationBuilder.CreateTable(
                name: "SubscriptionTypeHistory",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<long>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<long>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    SubscriptionTypeId = table.Column<long>(nullable: false),
                    ChargedMonthly = table.Column<decimal>(type: "decimal(17, 2)", nullable: false),
                    SubscriptionCharge = table.Column<decimal>(type: "decimal(17, 2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionTypeHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscriptionTypeHistory_SubscriptionType_SubscriptionTypeId",
                        column: x => x.SubscriptionTypeId,
                        principalTable: "SubscriptionType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionTypeHistory_SubscriptionTypeId",
                table: "SubscriptionTypeHistory",
                column: "SubscriptionTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubscriptionTypeHistory");

            migrationBuilder.AddColumn<decimal>(
                name: "ChargedMonthly",
                table: "CommissionHistory",
                type: "decimal(17, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "SubscriptionTypeId",
                table: "CommissionHistory",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_CommissionHistory_SubscriptionTypeId",
                table: "CommissionHistory",
                column: "SubscriptionTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommissionHistory_SubscriptionType_SubscriptionTypeId",
                table: "CommissionHistory",
                column: "SubscriptionTypeId",
                principalTable: "SubscriptionType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
