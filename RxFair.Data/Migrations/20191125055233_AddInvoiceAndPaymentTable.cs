using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class AddInvoiceAndPaymentTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommissionHistory_DistributorOrderCharge_OrderChargeId",
                table: "CommissionHistory");

            migrationBuilder.RenameColumn(
                name: "OrderChargeId",
                table: "CommissionHistory",
                newName: "DistributorId");

            migrationBuilder.RenameIndex(
                name: "IX_CommissionHistory_OrderChargeId",
                table: "CommissionHistory",
                newName: "IX_CommissionHistory_DistributorId");

            migrationBuilder.AddColumn<short>(
                name: "CommissionStatus",
                table: "CommissionHistory",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.CreateTable(
                name: "Invoice",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<long>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<long>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    CommissionId = table.Column<long>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(17, 2)", nullable: false),
                    InvoiceStatus = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoice_CommissionHistory_CommissionId",
                        column: x => x.CommissionId,
                        principalTable: "CommissionHistory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvoicePayment",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<long>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<long>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    InvoiceId = table.Column<long>(nullable: false),
                    PaymentAmount = table.Column<decimal>(type: "decimal(17, 2)", nullable: false),
                    PaymentStatus = table.Column<short>(nullable: false),
                    PaidBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoicePayment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoicePayment_Invoice_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_CommissionId",
                table: "Invoice",
                column: "CommissionId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoicePayment_InvoiceId",
                table: "InvoicePayment",
                column: "InvoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommissionHistory_Distributor_DistributorId",
                table: "CommissionHistory",
                column: "DistributorId",
                principalTable: "Distributor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommissionHistory_Distributor_DistributorId",
                table: "CommissionHistory");

            migrationBuilder.DropTable(
                name: "InvoicePayment");

            migrationBuilder.DropTable(
                name: "Invoice");

            migrationBuilder.DropColumn(
                name: "CommissionStatus",
                table: "CommissionHistory");

            migrationBuilder.RenameColumn(
                name: "DistributorId",
                table: "CommissionHistory",
                newName: "OrderChargeId");

            migrationBuilder.RenameIndex(
                name: "IX_CommissionHistory_DistributorId",
                table: "CommissionHistory",
                newName: "IX_CommissionHistory_OrderChargeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommissionHistory_DistributorOrderCharge_OrderChargeId",
                table: "CommissionHistory",
                column: "OrderChargeId",
                principalTable: "DistributorOrderCharge",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
