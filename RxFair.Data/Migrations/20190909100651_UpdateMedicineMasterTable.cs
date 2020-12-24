using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class UpdateMedicineMasterTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Package_Description_Code_Id",
                table: "MedicineMasters",
                nullable: true,
                oldClrType: typeof(long));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Package_Description_Code_Id",
                table: "MedicineMasters",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);
        }
    }
}
