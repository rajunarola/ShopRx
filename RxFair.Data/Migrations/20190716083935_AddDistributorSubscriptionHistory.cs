using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class AddDistributorSubscriptionHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UTCStandardOffset",
                table: "TimeZoneMaster",
                newName: "UtcStandardOffset");

            migrationBuilder.RenameColumn(
                name: "UTCDaylightOffset",
                table: "TimeZoneMaster",
                newName: "UtcDaylightOffset");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "DistributorSubscription",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DistributorSubscriptionHistory",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<long>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<long>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    DistributorId = table.Column<long>(nullable: false),
                    SubscriptionTypeId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DistributorSubscriptionHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DistributorSubscriptionHistory_Distributor_DistributorId",
                        column: x => x.DistributorId,
                        principalTable: "Distributor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DistributorSubscriptionHistory_SubscriptionType_SubscriptionTypeId",
                        column: x => x.SubscriptionTypeId,
                        principalTable: "SubscriptionType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DistributorSubscriptionHistory_DistributorId",
                table: "DistributorSubscriptionHistory",
                column: "DistributorId");

            migrationBuilder.CreateIndex(
                name: "IX_DistributorSubscriptionHistory_SubscriptionTypeId",
                table: "DistributorSubscriptionHistory",
                column: "SubscriptionTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DistributorSubscriptionHistory");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "DistributorSubscription");

            migrationBuilder.RenameColumn(
                name: "UtcStandardOffset",
                table: "TimeZoneMaster",
                newName: "UTCStandardOffset");

            migrationBuilder.RenameColumn(
                name: "UtcDaylightOffset",
                table: "TimeZoneMaster",
                newName: "UTCDaylightOffset");
        }
    }
}
