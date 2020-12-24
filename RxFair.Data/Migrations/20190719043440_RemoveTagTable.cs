using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class RemoveTagTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogTags_Tags_BlogTagId",
                table: "BlogTags");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_BlogTags_BlogTagId",
                table: "BlogTags");

            migrationBuilder.DropColumn(
                name: "BlogTagId",
                table: "BlogTags");

            migrationBuilder.AddColumn<string>(
                name: "TagName",
                table: "BlogTags",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TagName",
                table: "BlogTags");

            migrationBuilder.AddColumn<long>(
                name: "BlogTagId",
                table: "BlogTags",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<long>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    ModifiedBy = table.Column<long>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    TagName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlogTags_BlogTagId",
                table: "BlogTags",
                column: "BlogTagId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogTags_Tags_BlogTagId",
                table: "BlogTags",
                column: "BlogTagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
