using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JWTApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class migrent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Deposit",
                table: "RealEstates",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepositString",
                table: "RealEstates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Rent",
                table: "RealEstates",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RentString",
                table: "RealEstates",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deposit",
                table: "RealEstates");

            migrationBuilder.DropColumn(
                name: "DepositString",
                table: "RealEstates");

            migrationBuilder.DropColumn(
                name: "Rent",
                table: "RealEstates");

            migrationBuilder.DropColumn(
                name: "RentString",
                table: "RealEstates");
        }
    }
}
