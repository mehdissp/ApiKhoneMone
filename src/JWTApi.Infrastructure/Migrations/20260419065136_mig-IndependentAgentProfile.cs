using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JWTApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class migIndependentAgentProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RealEstateAgentProfileId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RegisteredByAgentId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RegisteredByAgentId1",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "IndependentAgentProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessLicense = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    RegisteredAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndependentAgentProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IndependentAgentProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RealEstateAgentProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AgentCode = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    NationalCartNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    OfficeAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LicenseNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    LicenseExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaxSubAgents = table.Column<int>(type: "int", nullable: false),
                    CurrentSubAgentsCount = table.Column<int>(type: "int", nullable: false),
                    AgentRank = table.Column<int>(type: "int", nullable: false),
                    SuccessFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RealEstateAgentProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RealEstateAgentProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_RealEstateAgentProfileId",
                table: "Users",
                column: "RealEstateAgentProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RegisteredByAgentId1",
                table: "Users",
                column: "RegisteredByAgentId1");

            migrationBuilder.CreateIndex(
                name: "IX_IndependentAgentProfiles_UserId",
                table: "IndependentAgentProfiles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RealEstateAgentProfiles_UserId",
                table: "RealEstateAgentProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_RealEstateAgentProfiles_RealEstateAgentProfileId",
                table: "Users",
                column: "RealEstateAgentProfileId",
                principalTable: "RealEstateAgentProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_RegisteredByAgentId1",
                table: "Users",
                column: "RegisteredByAgentId1",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_RealEstateAgentProfiles_RealEstateAgentProfileId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_RegisteredByAgentId1",
                table: "Users");

            migrationBuilder.DropTable(
                name: "IndependentAgentProfiles");

            migrationBuilder.DropTable(
                name: "RealEstateAgentProfiles");

            migrationBuilder.DropIndex(
                name: "IX_Users_RealEstateAgentProfileId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_RegisteredByAgentId1",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RealEstateAgentProfileId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RegisteredByAgentId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RegisteredByAgentId1",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");
        }
    }
}
