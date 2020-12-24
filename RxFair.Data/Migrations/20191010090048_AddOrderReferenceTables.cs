using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class AddOrderReferenceTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveryDate",
                table: "Order",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DeliveryStatus",
                table: "Order",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOverNight",
                table: "Order",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "OrderDate",
                table: "Order",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "OrderGrandTotal",
                table: "Order",
                type: "decimal(17, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OrderShippingTotal",
                table: "Order",
                type: "decimal(17, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OrderSubTotal",
                table: "Order",
                type: "decimal(17, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "PharmacyId",
                table: "Order",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DistributorOrderCharge",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<long>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<long>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    OrderId = table.Column<long>(nullable: false),
                    DistributorId = table.Column<long>(nullable: true),
                    SubTotal = table.Column<decimal>(type: "decimal(17, 2)", nullable: false),
                    ShippingTotal = table.Column<decimal>(type: "decimal(17, 2)", nullable: false),
                    GrandTotal = table.Column<decimal>(type: "decimal(17, 2)", nullable: false),
                    OverNightCharge = table.Column<decimal>(type: "decimal(17, 2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DistributorOrderCharge", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DistributorOrderCharge_Distributor_DistributorId",
                        column: x => x.DistributorId,
                        principalTable: "Distributor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DistributorOrderCharge_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DistributorOrder",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<long>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<long>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    OrderChargeId = table.Column<long>(nullable: false),
                    DistributorId = table.Column<long>(nullable: true),
                    MedicineId = table.Column<long>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    Price = table.Column<decimal>(type: "decimal(17, 2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(17, 2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DistributorOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DistributorOrder_Distributor_DistributorId",
                        column: x => x.DistributorId,
                        principalTable: "Distributor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DistributorOrder_MedicineMasters_MedicineId",
                        column: x => x.MedicineId,
                        principalTable: "MedicineMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DistributorOrder_DistributorOrderCharge_OrderChargeId",
                        column: x => x.OrderChargeId,
                        principalTable: "DistributorOrderCharge",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Order_PharmacyId",
                table: "Order",
                column: "PharmacyId");

            migrationBuilder.CreateIndex(
                name: "IX_DistributorOrder_DistributorId",
                table: "DistributorOrder",
                column: "DistributorId");

            migrationBuilder.CreateIndex(
                name: "IX_DistributorOrder_MedicineId",
                table: "DistributorOrder",
                column: "MedicineId");

            migrationBuilder.CreateIndex(
                name: "IX_DistributorOrder_OrderChargeId",
                table: "DistributorOrder",
                column: "OrderChargeId");

            migrationBuilder.CreateIndex(
                name: "IX_DistributorOrderCharge_DistributorId",
                table: "DistributorOrderCharge",
                column: "DistributorId");

            migrationBuilder.CreateIndex(
                name: "IX_DistributorOrderCharge_OrderId",
                table: "DistributorOrderCharge",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Pharmacy_PharmacyId",
                table: "Order",
                column: "PharmacyId",
                principalTable: "Pharmacy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Pharmacy_PharmacyId",
                table: "Order");

            migrationBuilder.DropTable(
                name: "DistributorOrder");

            migrationBuilder.DropTable(
                name: "DistributorOrderCharge");

            migrationBuilder.DropIndex(
                name: "IX_Order_PharmacyId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "DeliveryDate",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "DeliveryStatus",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "IsOverNight",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "OrderDate",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "OrderGrandTotal",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "OrderShippingTotal",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "OrderSubTotal",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "PharmacyId",
                table: "Order");
        }
    }
}
