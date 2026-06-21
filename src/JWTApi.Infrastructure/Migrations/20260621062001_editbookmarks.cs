using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JWTApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class editbookmarks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookMarks_RealEstatesRents_RealEstatesRentId",
                table: "BookMarks");

            migrationBuilder.DropIndex(
                name: "IX_BookMarks_RealEstatesRentId",
                table: "BookMarks");

            migrationBuilder.DropColumn(
                name: "RealEstatesRentId",
                table: "BookMarks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RealEstatesRentId",
                table: "BookMarks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookMarks_RealEstatesRentId",
                table: "BookMarks",
                column: "RealEstatesRentId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookMarks_RealEstatesRents_RealEstatesRentId",
                table: "BookMarks",
                column: "RealEstatesRentId",
                principalTable: "RealEstatesRents",
                principalColumn: "Id");
        }
    }
}
