using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class UpdateErrorLogTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Controller",
                table: "ErrorLog");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ErrorLog");

            migrationBuilder.DropColumn(
                name: "IsErrorSolve",
                table: "ErrorLog");

            migrationBuilder.DropColumn(
                name: "Method",
                table: "ErrorLog");

            migrationBuilder.RenameColumn(
                name: "ErrorMessage",
                table: "ErrorLog",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "ErrorInnerExceptionMessage",
                table: "ErrorLog",
                newName: "TargetSite");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "ErrorLog",
                newName: "LogDate");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ErrorLog",
                newName: "LogId");

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "ErrorLog",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "ErrorLog",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "ErrorLog",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Stack",
                table: "ErrorLog",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Message",
                table: "ErrorLog");

            migrationBuilder.DropColumn(
                name: "Path",
                table: "ErrorLog");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "ErrorLog");

            migrationBuilder.DropColumn(
                name: "Stack",
                table: "ErrorLog");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "ErrorLog",
                newName: "ErrorMessage");

            migrationBuilder.RenameColumn(
                name: "TargetSite",
                table: "ErrorLog",
                newName: "ErrorInnerExceptionMessage");

            migrationBuilder.RenameColumn(
                name: "LogDate",
                table: "ErrorLog",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "LogId",
                table: "ErrorLog",
                newName: "Id");

            migrationBuilder.AddColumn<string>(
                name: "Controller",
                table: "ErrorLog",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ErrorLog",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsErrorSolve",
                table: "ErrorLog",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Method",
                table: "ErrorLog",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
