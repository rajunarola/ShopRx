using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class AddUploadedXRxExtMedicineTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DistributorId",
                table: "MedicinePriceMasters",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "MedicineMasters",
                maxLength: 300,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RxExtDbMedicines",
                columns: table => new
                {
                    Row = table.Column<long>(name: "Row#", nullable: false),
                    NDC_UPC_HRI = table.Column<string>(maxLength: 11, nullable: false),
                    Old_NDC_UPC_HRI = table.Column<string>(maxLength: 11, nullable: true),
                    New_NDC_UPC_HRI = table.Column<string>(maxLength: 11, nullable: true),
                    Drug_Descriptor_Identifier = table.Column<int>(nullable: true),
                    Generic_Product_Identifier = table.Column<string>(maxLength: 14, nullable: true),
                    Generic_Product_Packaging_Code = table.Column<string>(maxLength: 8, nullable: true),
                    Drug_Name = table.Column<string>(maxLength: 30, nullable: true),
                    Strength = table.Column<string>(maxLength: 15, nullable: true),
                    Strength_Unit_of_Measure = table.Column<string>(maxLength: 10, nullable: true),
                    Dosage_Form = table.Column<string>(maxLength: 4, nullable: true),
                    Brand_Name_Code = table.Column<string>(maxLength: 1, nullable: true),
                    Package_Size = table.Column<float>(nullable: true),
                    Package_Size_Unit_of_Measure = table.Column<string>(maxLength: 2, nullable: true),
                    Package_Quantity = table.Column<int>(nullable: true),
                    Unit_DoseUnit_of_Use_Package = table.Column<string>(maxLength: 1, nullable: true),
                    Package_Description_Code = table.Column<string>(maxLength: 2, nullable: true),
                    Manufacturers_Labeler_Name = table.Column<string>(maxLength: 50, nullable: true),
                    GPPC_Price_Code = table.Column<string>(maxLength: 1, nullable: true),
                    GPPC_Unit_Price = table.Column<float>(nullable: true),
                    Image_Filename = table.Column<string>(maxLength: 20, nullable: true),
                    Last_Change_Date = table.Column<long>(nullable: true),
                    IsDuplicate = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RxExtDbMedicines", x => x.Row);
                });

            migrationBuilder.CreateTable(
                name: "UploadedMedicines",
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
                    Drug_Name = table.Column<string>(maxLength: 30, nullable: true),
                    Strength = table.Column<string>(maxLength: 15, nullable: true),
                    Strength_Id = table.Column<long>(nullable: false),
                    UploadedMedicine_Strength_Id = table.Column<long>(nullable: true),
                    Dosage_Form_Id = table.Column<long>(nullable: false),
                    UploadedMedicine_Dosage_Form_Id = table.Column<long>(nullable: true),
                    Brand_Id = table.Column<long>(nullable: false),
                    UploadedMedicine_Brand_Id = table.Column<long>(nullable: true),
                    Package_Size = table.Column<float>(nullable: true),
                    Package_Size_Id = table.Column<long>(nullable: false),
                    UploadedMedicine_Package_Size_Id = table.Column<long>(nullable: true),
                    Package_Quantity = table.Column<int>(nullable: true),
                    Unit_Size_Id = table.Column<long>(nullable: false),
                    UploadedMedicine_Unit_Size_Id = table.Column<long>(nullable: true),
                    Package_Description_Code_Id = table.Column<long>(nullable: false),
                    UploadedMedicine_Package_Description_Code_Id = table.Column<long>(nullable: true),
                    Manufacturer_Id = table.Column<long>(nullable: false),
                    UploadedMedicine_Manufacturer_Id = table.Column<long>(nullable: true),
                    MedicineImage = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    DistributorId = table.Column<long>(nullable: false),
                    IsApproved = table.Column<bool>(nullable: true),
                    Reason = table.Column<string>(maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadedMedicines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UploadedMedicines_Measurement_UploadedMedicine_Brand_Id",
                        column: x => x.UploadedMedicine_Brand_Id,
                        principalTable: "Measurement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UploadedMedicines_Distributor_DistributorId",
                        column: x => x.DistributorId,
                        principalTable: "Distributor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UploadedMedicines_DosageFormMaster_UploadedMedicine_Dosage_Form_Id",
                        column: x => x.UploadedMedicine_Dosage_Form_Id,
                        principalTable: "DosageFormMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UploadedMedicines_ManufacturerMaster_UploadedMedicine_Manufacturer_Id",
                        column: x => x.UploadedMedicine_Manufacturer_Id,
                        principalTable: "ManufacturerMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UploadedMedicines_Measurement_UploadedMedicine_Package_Description_Code_Id",
                        column: x => x.UploadedMedicine_Package_Description_Code_Id,
                        principalTable: "Measurement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UploadedMedicines_Measurement_UploadedMedicine_Package_Size_Id",
                        column: x => x.UploadedMedicine_Package_Size_Id,
                        principalTable: "Measurement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UploadedMedicines_Measurement_UploadedMedicine_Strength_Id",
                        column: x => x.UploadedMedicine_Strength_Id,
                        principalTable: "Measurement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UploadedMedicines_Measurement_UploadedMedicine_Unit_Size_Id",
                        column: x => x.UploadedMedicine_Unit_Size_Id,
                        principalTable: "Measurement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedicinePriceMasters_DistributorId",
                table: "MedicinePriceMasters",
                column: "DistributorId");

            migrationBuilder.CreateIndex(
                name: "IX_UploadedMedicines_UploadedMedicine_Brand_Id",
                table: "UploadedMedicines",
                column: "UploadedMedicine_Brand_Id");

            migrationBuilder.CreateIndex(
                name: "IX_UploadedMedicines_DistributorId",
                table: "UploadedMedicines",
                column: "DistributorId");

            migrationBuilder.CreateIndex(
                name: "IX_UploadedMedicines_UploadedMedicine_Dosage_Form_Id",
                table: "UploadedMedicines",
                column: "UploadedMedicine_Dosage_Form_Id");

            migrationBuilder.CreateIndex(
                name: "IX_UploadedMedicines_UploadedMedicine_Manufacturer_Id",
                table: "UploadedMedicines",
                column: "UploadedMedicine_Manufacturer_Id");

            migrationBuilder.CreateIndex(
                name: "IX_UploadedMedicines_UploadedMedicine_Package_Description_Code_Id",
                table: "UploadedMedicines",
                column: "UploadedMedicine_Package_Description_Code_Id");

            migrationBuilder.CreateIndex(
                name: "IX_UploadedMedicines_UploadedMedicine_Package_Size_Id",
                table: "UploadedMedicines",
                column: "UploadedMedicine_Package_Size_Id");

            migrationBuilder.CreateIndex(
                name: "IX_UploadedMedicines_UploadedMedicine_Strength_Id",
                table: "UploadedMedicines",
                column: "UploadedMedicine_Strength_Id");

            migrationBuilder.CreateIndex(
                name: "IX_UploadedMedicines_UploadedMedicine_Unit_Size_Id",
                table: "UploadedMedicines",
                column: "UploadedMedicine_Unit_Size_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicinePriceMasters_Distributor_DistributorId",
                table: "MedicinePriceMasters",
                column: "DistributorId",
                principalTable: "Distributor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicinePriceMasters_Distributor_DistributorId",
                table: "MedicinePriceMasters");

            migrationBuilder.DropTable(
                name: "RxExtDbMedicines");

            migrationBuilder.DropTable(
                name: "UploadedMedicines");

            migrationBuilder.DropIndex(
                name: "IX_MedicinePriceMasters_DistributorId",
                table: "MedicinePriceMasters");

            migrationBuilder.DropColumn(
                name: "DistributorId",
                table: "MedicinePriceMasters");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "MedicineMasters");
        }
    }
}
