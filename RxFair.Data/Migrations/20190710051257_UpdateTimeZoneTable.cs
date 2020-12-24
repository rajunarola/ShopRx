using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class UpdateTimeZoneTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeZone",
                table: "TimeZoneMaster");

            migrationBuilder.RenameColumn(
                name: "TimeZoneName",
                table: "TimeZoneMaster",
                newName: "StandardName");

            migrationBuilder.AddColumn<string>(
                name: "DaylightName",
                table: "TimeZoneMaster",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "TimeZoneMaster",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsDaylight",
                table: "TimeZoneMaster",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "UTCDaylightOffset",
                table: "TimeZoneMaster",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UTCStandardOffset",
                table: "TimeZoneMaster",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DaylightName",
                table: "TimeZoneMaster");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "TimeZoneMaster");

            migrationBuilder.DropColumn(
                name: "IsDaylight",
                table: "TimeZoneMaster");

            migrationBuilder.DropColumn(
                name: "UTCDaylightOffset",
                table: "TimeZoneMaster");

            migrationBuilder.DropColumn(
                name: "UTCStandardOffset",
                table: "TimeZoneMaster");

            migrationBuilder.RenameColumn(
                name: "StandardName",
                table: "TimeZoneMaster",
                newName: "TimeZoneName");

            migrationBuilder.AddColumn<string>(
                name: "TimeZone",
                table: "TimeZoneMaster",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
