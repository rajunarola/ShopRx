using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class RemoveExtraFiledsFromMedicineTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "MedicinePrices");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "MedicinePrices");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "MedicinePrices");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "MedicineDetails");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "MedicineDetails");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "MedicineDetails");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                table: "MedicinePrices",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "MedicinePrices",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "MedicinePrices",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                table: "MedicineDetails",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "MedicineDetails",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "MedicineDetails",
                nullable: false,
                defaultValue: false);
        }
    }
}
