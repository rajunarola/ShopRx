using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class AddRedeemRequestTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RedeemRequest",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<long>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<long>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    RewardProductId = table.Column<long>(nullable: false),
                    PharmacyId = table.Column<long>(nullable: false),
                    IsApprove = table.Column<bool>(nullable: false),
                    DeliveryStatus = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RedeemRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RedeemRequest_Pharmacy_PharmacyId",
                        column: x => x.PharmacyId,
                        principalTable: "Pharmacy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RedeemRequest_RewardProduct_RewardProductId",
                        column: x => x.RewardProductId,
                        principalTable: "RewardProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RedeemRequest_PharmacyId",
                table: "RedeemRequest",
                column: "PharmacyId");

            migrationBuilder.CreateIndex(
                name: "IX_RedeemRequest_RewardProductId",
                table: "RedeemRequest",
                column: "RewardProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RedeemRequest");
        }
    }
}
