using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JWTApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class migwarningscat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Warnings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Warnings_CategoryId",
                table: "Warnings",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Warnings_Categories_CategoryId",
                table: "Warnings",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Warnings_Categories_CategoryId",
                table: "Warnings");

            migrationBuilder.DropIndex(
                name: "IX_Warnings_CategoryId",
                table: "Warnings");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Warnings");
        }
    }
}
