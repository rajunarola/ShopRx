using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class UpdateBlogTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "Blog");

            migrationBuilder.AddColumn<long>(
                name: "AuthorId",
                table: "Blog",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Blog_AuthorId",
                table: "Blog",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Blog_AspNetUsers_AuthorId",
                table: "Blog",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blog_AspNetUsers_AuthorId",
                table: "Blog");

            migrationBuilder.DropIndex(
                name: "IX_Blog_AuthorId",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Blog");

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Blog",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
