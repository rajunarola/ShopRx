using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class UpdateTimeZoneMasterTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TimeZoneId",
                table: "TimeZoneMaster",
                maxLength: 250,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeZoneId",
                table: "TimeZoneMaster");
        }
    }
}
