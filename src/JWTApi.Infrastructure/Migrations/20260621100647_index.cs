using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JWTApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class index : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RealEstates_RegionId",
                table: "RealEstates");

            migrationBuilder.DropIndex(
                name: "IX_RealEstates_UserId",
                table: "RealEstates");

            migrationBuilder.CreateIndex(
                name: "IX_RealEstates_Main",
                table: "RealEstates",
                columns: new[] { "Status", "IsDeleted", "CategoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_RealEstates_Region",
                table: "RealEstates",
                columns: new[] { "RegionId", "Status", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_RealEstates_User",
                table: "RealEstates",
                columns: new[] { "UserId", "Status", "IsDeleted" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RealEstates_Main",
                table: "RealEstates");

            migrationBuilder.DropIndex(
                name: "IX_RealEstates_Region",
                table: "RealEstates");

            migrationBuilder.DropIndex(
                name: "IX_RealEstates_User",
                table: "RealEstates");

            migrationBuilder.CreateIndex(
                name: "IX_RealEstates_RegionId",
                table: "RealEstates",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_RealEstates_UserId",
                table: "RealEstates",
                column: "UserId");
        }
    }
}
