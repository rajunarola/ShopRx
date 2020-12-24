using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class UpdateMedicinePriceMaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Medicine");

            migrationBuilder.DropTable(
                name: "MedicineDetails");

            migrationBuilder.DropTable(
                name: "MedicinePrices");

            migrationBuilder.AddColumn<bool>(
                name: "InStock",
                table: "MedicinePriceMasters",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsContracted",
                table: "MedicinePriceMasters",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsShortDated",
                table: "MedicinePriceMasters",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InStock",
                table: "MedicinePriceMasters");

            migrationBuilder.DropColumn(
                name: "IsContracted",
                table: "MedicinePriceMasters");

            migrationBuilder.DropColumn(
                name: "IsShortDated",
                table: "MedicinePriceMasters");

            migrationBuilder.CreateTable(
                name: "Medicine",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<long>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "(getdate())"),
                    Description = table.Column<string>(nullable: true),
                    Drug_Descriptor_Identifier = table.Column<int>(nullable: true),
                    Generic_Product_Identifier = table.Column<string>(maxLength: 14, nullable: true),
                    Generic_Product_Packaging_Code = table.Column<string>(maxLength: 8, nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsNDC = table.Column<bool>(nullable: false),
                    Last_Change_Date = table.Column<long>(nullable: true),
                    Manufacturers_Labeler_Name = table.Column<string>(maxLength: 50, nullable: true),
                    MedicineImage = table.Column<string>(maxLength: 20, nullable: true),
                    ModifiedBy = table.Column<long>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true, defaultValueSql: "(getdate())"),
                    NDC_UPC_HRI = table.Column<string>(unicode: false, maxLength: 11, nullable: false),
                    New_NDC_UPC_HRI = table.Column<string>(unicode: false, maxLength: 11, nullable: true),
                    Old_NDC_UPC_HRI = table.Column<string>(unicode: false, maxLength: 11, nullable: true),
                    Package_Description_Code = table.Column<string>(maxLength: 2, nullable: true),
                    Package_Quantity = table.Column<int>(nullable: true),
                    Package_Size = table.Column<float>(nullable: true),
                    Package_Size_Unit_of_Measure = table.Column<string>(maxLength: 2, nullable: true),
                    Unit_DoseUnit_of_Use_Package = table.Column<string>(maxLength: 1, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicine", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MedicineDetails",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Brand_Name_Code = table.Column<string>(maxLength: 1, nullable: true),
                    Dosage_Form = table.Column<string>(maxLength: 4, nullable: true),
                    Drug_Descriptor_Identifier = table.Column<int>(nullable: true),
                    Drug_Name = table.Column<string>(maxLength: 30, nullable: true),
                    Generic_Product_Identifier = table.Column<string>(maxLength: 14, nullable: true),
                    ModifiedBy = table.Column<long>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true, defaultValueSql: "(getdate())"),
                    Strength = table.Column<string>(maxLength: 15, nullable: true),
                    Strength_Unit_of_Measure = table.Column<string>(maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicineDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MedicinePrices",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AWPPackage_Price = table.Column<float>(nullable: true),
                    AWPUnit_Price = table.Column<float>(nullable: true),
                    AWPUnit_Price_Extended = table.Column<float>(nullable: true),
                    Effective_Date = table.Column<long>(nullable: true),
                    Generic_Product_Packaging_Code = table.Column<string>(maxLength: 8, nullable: true),
                    GPPC_Price_Code = table.Column<string>(unicode: false, maxLength: 1, nullable: true),
                    GPPC_Unit_Price = table.Column<float>(nullable: true),
                    ModifiedBy = table.Column<long>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true, defaultValueSql: "(getdate())"),
                    NDC_UPC_HRI = table.Column<string>(unicode: false, maxLength: 11, nullable: false),
                    WACPackage_Price = table.Column<float>(nullable: true),
                    WACUnit_Price = table.Column<float>(nullable: true),
                    WACUnit_Price_Extended = table.Column<float>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicinePrices", x => x.Id);
                });
        }
    }
}
