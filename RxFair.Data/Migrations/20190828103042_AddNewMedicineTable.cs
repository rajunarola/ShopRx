using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class AddNewMedicineTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsApprove",
                table: "RedeemRequest",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.CreateTable(
                name: "MedicineMasters",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<long>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<long>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    NDC_UPC_HRI = table.Column<string>(maxLength: 11, nullable: false),
                    Old_NDC_UPC_HRI = table.Column<string>(maxLength: 11, nullable: true),
                    New_NDC_UPC_HRI = table.Column<string>(maxLength: 11, nullable: true),
                    Drug_Descriptor_Identifier = table.Column<int>(nullable: true),
                    Generic_Product_Identifier = table.Column<string>(maxLength: 14, nullable: true),
                    Generic_Product_Packaging_Code = table.Column<string>(maxLength: 8, nullable: true),
                    Drug_Name = table.Column<string>(maxLength: 30, nullable: true),
                    Strength = table.Column<string>(maxLength: 15, nullable: true),
                    Strength_Id = table.Column<long>(nullable: false),
                    MedicineMaster_Strength_Id = table.Column<long>(nullable: true),
                    Dosage_Form_Id = table.Column<long>(nullable: false),
                    MedicineMaster_Dosage_Form_Id = table.Column<long>(nullable: true),
                    Brand_Id = table.Column<long>(nullable: false),
                    MedicineMaster_Brand_Id = table.Column<long>(nullable: true),
                    Package_Size = table.Column<float>(nullable: true),
                    Package_Size_Id = table.Column<long>(nullable: false),
                    MedicineMaster_Package_Size_Id = table.Column<long>(nullable: true),
                    Package_Quantity = table.Column<int>(nullable: true),
                    Unit_Size_Id = table.Column<long>(nullable: false),
                    MedicineMaster_Unit_Size_Id = table.Column<long>(nullable: true),
                    Package_Description_Code_Id = table.Column<long>(nullable: false),
                    MedicineMaster_Package_Description_Code_Id = table.Column<long>(nullable: true),
                    Manufacturer_Id = table.Column<long>(nullable: false),
                    MedicineMaster_Manufacturer_Id = table.Column<long>(nullable: true),
                    MedicineImage = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Last_Change_Date = table.Column<long>(nullable: true),
                    IsNDC = table.Column<bool>(nullable: false),
                    DistributorId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicineMasters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicineMasters_Measurement_MedicineMaster_Brand_Id",
                        column: x => x.MedicineMaster_Brand_Id,
                        principalTable: "Measurement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MedicineMasters_Distributor_DistributorId",
                        column: x => x.DistributorId,
                        principalTable: "Distributor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MedicineMasters_DosageFormMaster_MedicineMaster_Dosage_Form_Id",
                        column: x => x.MedicineMaster_Dosage_Form_Id,
                        principalTable: "DosageFormMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MedicineMasters_ManufacturerMaster_MedicineMaster_Manufacturer_Id",
                        column: x => x.MedicineMaster_Manufacturer_Id,
                        principalTable: "ManufacturerMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MedicineMasters_Measurement_MedicineMaster_Package_Description_Code_Id",
                        column: x => x.MedicineMaster_Package_Description_Code_Id,
                        principalTable: "Measurement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MedicineMasters_Measurement_MedicineMaster_Package_Size_Id",
                        column: x => x.MedicineMaster_Package_Size_Id,
                        principalTable: "Measurement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MedicineMasters_Measurement_MedicineMaster_Strength_Id",
                        column: x => x.MedicineMaster_Strength_Id,
                        principalTable: "Measurement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MedicineMasters_Measurement_MedicineMaster_Unit_Size_Id",
                        column: x => x.MedicineMaster_Unit_Size_Id,
                        principalTable: "Measurement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MedicinePriceMasters",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<long>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<long>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    MedicineId = table.Column<long>(nullable: false),
                    NDC_UPC_HRI = table.Column<string>(maxLength: 11, nullable: false),
                    Generic_Product_Packaging_Code = table.Column<string>(maxLength: 8, nullable: true),
                    GPPC_Price_Code = table.Column<string>(maxLength: 1, nullable: true),
                    GPPC_Unit_Price = table.Column<float>(nullable: true),
                    AWPUnit_Price = table.Column<float>(nullable: true),
                    AWPUnit_Price_Extended = table.Column<float>(nullable: true),
                    AWPPackage_Price = table.Column<float>(nullable: true),
                    WACUnit_Price = table.Column<float>(nullable: true),
                    WACUnit_Price_Extended = table.Column<float>(nullable: true),
                    WACPackage_Price = table.Column<float>(nullable: true),
                    Effective_Date = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicinePriceMasters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicinePriceMasters_MedicineMasters_MedicineId",
                        column: x => x.MedicineId,
                        principalTable: "MedicineMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedicineMasters_MedicineMaster_Brand_Id",
                table: "MedicineMasters",
                column: "MedicineMaster_Brand_Id");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineMasters_DistributorId",
                table: "MedicineMasters",
                column: "DistributorId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineMasters_MedicineMaster_Dosage_Form_Id",
                table: "MedicineMasters",
                column: "MedicineMaster_Dosage_Form_Id");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineMasters_MedicineMaster_Manufacturer_Id",
                table: "MedicineMasters",
                column: "MedicineMaster_Manufacturer_Id");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineMasters_MedicineMaster_Package_Description_Code_Id",
                table: "MedicineMasters",
                column: "MedicineMaster_Package_Description_Code_Id");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineMasters_MedicineMaster_Package_Size_Id",
                table: "MedicineMasters",
                column: "MedicineMaster_Package_Size_Id");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineMasters_MedicineMaster_Strength_Id",
                table: "MedicineMasters",
                column: "MedicineMaster_Strength_Id");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineMasters_MedicineMaster_Unit_Size_Id",
                table: "MedicineMasters",
                column: "MedicineMaster_Unit_Size_Id");

            migrationBuilder.CreateIndex(
                name: "IX_MedicinePriceMasters_MedicineId",
                table: "MedicinePriceMasters",
                column: "MedicineId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedicinePriceMasters");

            migrationBuilder.DropTable(
                name: "MedicineMasters");

            migrationBuilder.AlterColumn<bool>(
                name: "IsApprove",
                table: "RedeemRequest",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);
        }
    }
}
