using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class AddNewColumnUploadedMedicine : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MedicineImage",
                table: "UploadedMedicines",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Upc",
                table: "UploadedMedicines",
                maxLength: 11,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MedicineImage",
                table: "UploadedMedicines");

            migrationBuilder.DropColumn(
                name: "Upc",
                table: "UploadedMedicines");
        }
    }
}
