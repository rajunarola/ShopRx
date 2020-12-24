using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class UpdateUploadedMedicine : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UploadedMedicines_Measurement_UploadedMedicine_Brand_Id",
                table: "UploadedMedicines");

            migrationBuilder.DropForeignKey(
                name: "FK_UploadedMedicines_DosageFormMaster_UploadedMedicine_Dosage_Form_Id",
                table: "UploadedMedicines");

            migrationBuilder.DropForeignKey(
                name: "FK_UploadedMedicines_ManufacturerMaster_UploadedMedicine_Manufacturer_Id",
                table: "UploadedMedicines");

            migrationBuilder.DropForeignKey(
                name: "FK_UploadedMedicines_Measurement_UploadedMedicine_Package_Description_Code_Id",
                table: "UploadedMedicines");

            migrationBuilder.DropForeignKey(
                name: "FK_UploadedMedicines_Measurement_UploadedMedicine_Package_Size_Id",
                table: "UploadedMedicines");

            migrationBuilder.DropForeignKey(
                name: "FK_UploadedMedicines_Measurement_UploadedMedicine_Strength_Id",
                table: "UploadedMedicines");

            migrationBuilder.DropForeignKey(
                name: "FK_UploadedMedicines_Measurement_UploadedMedicine_Unit_Size_Id",
                table: "UploadedMedicines");

            migrationBuilder.DropIndex(
                name: "IX_UploadedMedicines_UploadedMedicine_Brand_Id",
                table: "UploadedMedicines");

            migrationBuilder.DropIndex(
                name: "IX_UploadedMedicines_UploadedMedicine_Dosage_Form_Id",
                table: "UploadedMedicines");

            migrationBuilder.DropIndex(
                name: "IX_UploadedMedicines_UploadedMedicine_Manufacturer_Id",
                table: "UploadedMedicines");

            migrationBuilder.DropIndex(
                name: "IX_UploadedMedicines_UploadedMedicine_Package_Description_Code_Id",
                table: "UploadedMedicines");

            migrationBuilder.DropIndex(
                name: "IX_UploadedMedicines_UploadedMedicine_Package_Size_Id",
                table: "UploadedMedicines");

            migrationBuilder.DropIndex(
                name: "IX_UploadedMedicines_UploadedMedicine_Strength_Id",
                table: "UploadedMedicines");

            migrationBuilder.DropIndex(
                name: "IX_UploadedMedicines_UploadedMedicine_Unit_Size_Id",
                table: "UploadedMedicines");

            migrationBuilder.DropColumn(
                name: "Brand_Id",
                table: "UploadedMedicines");

            migrationBuilder.DropColumn(
                name: "UploadedMedicine_Brand_Id",
                table: "UploadedMedicines");

            migrationBuilder.DropColumn(
                name: "Dosage_Form_Id",
                table: "UploadedMedicines");

            migrationBuilder.DropColumn(
                name: "UploadedMedicine_Dosage_Form_Id",
                table: "UploadedMedicines");

            migrationBuilder.DropColumn(
                name: "Manufacturer_Id",
                table: "UploadedMedicines");

            migrationBuilder.DropColumn(
                name: "UploadedMedicine_Manufacturer_Id",
                table: "UploadedMedicines");

            migrationBuilder.DropColumn(
                name: "Package_Description_Code_Id",
                table: "UploadedMedicines");

            migrationBuilder.DropColumn(
                name: "Package_Quantity",
                table: "UploadedMedicines");

            migrationBuilder.DropColumn(
                name: "Package_Size_Id",
                table: "UploadedMedicines");

            migrationBuilder.DropColumn(
                name: "UploadedMedicine_Package_Description_Code_Id",
                table: "UploadedMedicines");

            migrationBuilder.DropColumn(
                name: "UploadedMedicine_Package_Size_Id",
                table: "UploadedMedicines");

            migrationBuilder.DropColumn(
                name: "Strength_Id",
                table: "UploadedMedicines");

            migrationBuilder.DropColumn(
                name: "UploadedMedicine_Strength_Id",
                table: "UploadedMedicines");

            migrationBuilder.DropColumn(
                name: "Unit_Size_Id",
                table: "UploadedMedicines");

            migrationBuilder.DropColumn(
                name: "UploadedMedicine_Unit_Size_Id",
                table: "UploadedMedicines");

            migrationBuilder.RenameColumn(
                name: "Package_Size",
                table: "UploadedMedicines",
                newName: "PackageSize");

            migrationBuilder.RenameColumn(
                name: "NDC_UPC_HRI",
                table: "UploadedMedicines",
                newName: "Ndc");

            migrationBuilder.RenameColumn(
                name: "MedicineImage",
                table: "UploadedMedicines",
                newName: "Manufacturer");

            migrationBuilder.RenameColumn(
                name: "Drug_Name",
                table: "UploadedMedicines",
                newName: "MedicineName");

            migrationBuilder.AddColumn<string>(
                name: "Brand",
                table: "UploadedMedicines",
                maxLength: 4,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "UploadedMedicines",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DosageForm",
                table: "UploadedMedicines",
                maxLength: 4,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "UploadedMedicines",
                maxLength: 1,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Brand",
                table: "UploadedMedicines");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "UploadedMedicines");

            migrationBuilder.DropColumn(
                name: "DosageForm",
                table: "UploadedMedicines");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "UploadedMedicines");

            migrationBuilder.RenameColumn(
                name: "PackageSize",
                table: "UploadedMedicines",
                newName: "Package_Size");

            migrationBuilder.RenameColumn(
                name: "Ndc",
                table: "UploadedMedicines",
                newName: "NDC_UPC_HRI");

            migrationBuilder.RenameColumn(
                name: "MedicineName",
                table: "UploadedMedicines",
                newName: "Drug_Name");

            migrationBuilder.RenameColumn(
                name: "Manufacturer",
                table: "UploadedMedicines",
                newName: "MedicineImage");

            migrationBuilder.AddColumn<long>(
                name: "Brand_Id",
                table: "UploadedMedicines",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "UploadedMedicine_Brand_Id",
                table: "UploadedMedicines",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Dosage_Form_Id",
                table: "UploadedMedicines",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "UploadedMedicine_Dosage_Form_Id",
                table: "UploadedMedicines",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Manufacturer_Id",
                table: "UploadedMedicines",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "UploadedMedicine_Manufacturer_Id",
                table: "UploadedMedicines",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Package_Description_Code_Id",
                table: "UploadedMedicines",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "Package_Quantity",
                table: "UploadedMedicines",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Package_Size_Id",
                table: "UploadedMedicines",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "UploadedMedicine_Package_Description_Code_Id",
                table: "UploadedMedicines",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UploadedMedicine_Package_Size_Id",
                table: "UploadedMedicines",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Strength_Id",
                table: "UploadedMedicines",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "UploadedMedicine_Strength_Id",
                table: "UploadedMedicines",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Unit_Size_Id",
                table: "UploadedMedicines",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "UploadedMedicine_Unit_Size_Id",
                table: "UploadedMedicines",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UploadedMedicines_UploadedMedicine_Brand_Id",
                table: "UploadedMedicines",
                column: "UploadedMedicine_Brand_Id");

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
                name: "FK_UploadedMedicines_Measurement_UploadedMedicine_Brand_Id",
                table: "UploadedMedicines",
                column: "UploadedMedicine_Brand_Id",
                principalTable: "Measurement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UploadedMedicines_DosageFormMaster_UploadedMedicine_Dosage_Form_Id",
                table: "UploadedMedicines",
                column: "UploadedMedicine_Dosage_Form_Id",
                principalTable: "DosageFormMaster",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UploadedMedicines_ManufacturerMaster_UploadedMedicine_Manufacturer_Id",
                table: "UploadedMedicines",
                column: "UploadedMedicine_Manufacturer_Id",
                principalTable: "ManufacturerMaster",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UploadedMedicines_Measurement_UploadedMedicine_Package_Description_Code_Id",
                table: "UploadedMedicines",
                column: "UploadedMedicine_Package_Description_Code_Id",
                principalTable: "Measurement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UploadedMedicines_Measurement_UploadedMedicine_Package_Size_Id",
                table: "UploadedMedicines",
                column: "UploadedMedicine_Package_Size_Id",
                principalTable: "Measurement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UploadedMedicines_Measurement_UploadedMedicine_Strength_Id",
                table: "UploadedMedicines",
                column: "UploadedMedicine_Strength_Id",
                principalTable: "Measurement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UploadedMedicines_Measurement_UploadedMedicine_Unit_Size_Id",
                table: "UploadedMedicines",
                column: "UploadedMedicine_Unit_Size_Id",
                principalTable: "Measurement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
