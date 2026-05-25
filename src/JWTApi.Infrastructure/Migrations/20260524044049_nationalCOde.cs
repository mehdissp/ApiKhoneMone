using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JWTApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class nationalCOde : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NationalCode",
                table: "Users",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NationalCode",
                table: "Users");
        }
    }
}
