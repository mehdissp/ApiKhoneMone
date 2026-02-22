using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JWTApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class migeditrealesatete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsHasElevator",
                table: "RealEstatesRents",
                type: "bit",
                nullable: false,
                defaultValueSql: "0");

            migrationBuilder.AddColumn<bool>(
                name: "IsHasParking",
                table: "RealEstatesRents",
                type: "bit",
                nullable: false,
                defaultValueSql: "0");

            migrationBuilder.AddColumn<bool>(
                name: "IsHasPool",
                table: "RealEstatesRents",
                type: "bit",
                nullable: false,
                defaultValueSql: "0");

            migrationBuilder.AddColumn<bool>(
                name: "IsHasStoreRoom",
                table: "RealEstatesRents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHasElevator",
                table: "RealEstates",
                type: "bit",
                nullable: false,
                defaultValueSql: "0");

            migrationBuilder.AddColumn<bool>(
                name: "IsHasParking",
                table: "RealEstates",
                type: "bit",
                nullable: false,
                defaultValueSql: "0");

            migrationBuilder.AddColumn<bool>(
                name: "IsHasPool",
                table: "RealEstates",
                type: "bit",
                nullable: false,
                defaultValueSql: "0");

            migrationBuilder.AddColumn<bool>(
                name: "IsHasStoreRoom",
                table: "RealEstates",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsHasElevator",
                table: "RealEstatesRents");

            migrationBuilder.DropColumn(
                name: "IsHasParking",
                table: "RealEstatesRents");

            migrationBuilder.DropColumn(
                name: "IsHasPool",
                table: "RealEstatesRents");

            migrationBuilder.DropColumn(
                name: "IsHasStoreRoom",
                table: "RealEstatesRents");

            migrationBuilder.DropColumn(
                name: "IsHasElevator",
                table: "RealEstates");

            migrationBuilder.DropColumn(
                name: "IsHasParking",
                table: "RealEstates");

            migrationBuilder.DropColumn(
                name: "IsHasPool",
                table: "RealEstates");

            migrationBuilder.DropColumn(
                name: "IsHasStoreRoom",
                table: "RealEstates");
        }
    }
}
