using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class AddTableMedicinePriceHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MedicinePriceHistory",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MedicinePriceId = table.Column<long>(nullable: false),
                    AWPUnit_Price = table.Column<float>(nullable: true),
                    AWPUnit_Price_Extended = table.Column<float>(nullable: true),
                    AWPPackage_Price = table.Column<float>(nullable: true),
                    WACUnit_Price = table.Column<float>(nullable: true),
                    WACUnit_Price_Extended = table.Column<float>(nullable: true),
                    WACPackage_Price = table.Column<float>(nullable: true),
                    IsShortDated = table.Column<bool>(nullable: false),
                    IsContracted = table.Column<bool>(nullable: false),
                    InStock = table.Column<bool>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicinePriceHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicinePriceHistory_MedicinePriceMasters_MedicinePriceId",
                        column: x => x.MedicinePriceId,
                        principalTable: "MedicinePriceMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedicinePriceHistory_MedicinePriceId",
                table: "MedicinePriceHistory",
                column: "MedicinePriceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedicinePriceHistory");
        }
    }
}
