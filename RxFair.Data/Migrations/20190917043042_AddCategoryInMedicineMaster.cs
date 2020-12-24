using Microsoft.EntityFrameworkCore.Migrations;

namespace RxFair.Data.Migrations
{
    public partial class AddCategoryInMedicineMaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CategoryId",
                table: "MedicineMasters",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedicineMasters_CategoryId",
                table: "MedicineMasters",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicineMasters_MedicineCategoryMaster_CategoryId",
                table: "MedicineMasters",
                column: "CategoryId",
                principalTable: "MedicineCategoryMaster",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicineMasters_MedicineCategoryMaster_CategoryId",
                table: "MedicineMasters");

            migrationBuilder.DropIndex(
                name: "IX_MedicineMasters_CategoryId",
                table: "MedicineMasters");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "MedicineMasters");
        }
    }
}
