using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JWTApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class migaddFacilitiesand : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "RealEstates",
                type: "bit",
                nullable: false,
                defaultValueSql: "0");

            migrationBuilder.CreateTable(
                name: "BookMarks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RealEstatesId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DescriptionRows = table.Column<string>(type: "nvarchar(550)", maxLength: 550, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookMarks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookMarks_RealEstates_RealEstatesId",
                        column: x => x.RealEstatesId,
                        principalTable: "RealEstates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RealEstates_Facilities",
                columns: table => new
                {
                    RealStatesId = table.Column<int>(type: "int", nullable: false),
                    FacilitiesId = table.Column<int>(type: "int", nullable: false),
                    RealEstatesId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RealEstates_Facilities", x => new { x.RealStatesId, x.FacilitiesId });
                    table.ForeignKey(
                        name: "FK_RealEstates_Facilities_Facilities_FacilitiesId",
                        column: x => x.FacilitiesId,
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RealEstates_Facilities_RealEstates_RealEstatesId",
                        column: x => x.RealEstatesId,
                        principalTable: "RealEstates",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RealEstates_SpecialFeatures",
                columns: table => new
                {
                    RealStatesId = table.Column<int>(type: "int", nullable: false),
                    SpecialFeatureId = table.Column<int>(type: "int", nullable: false),
                    RealEstatesId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RealEstates_SpecialFeatures", x => new { x.RealStatesId, x.SpecialFeatureId });
                    table.ForeignKey(
                        name: "FK_RealEstates_SpecialFeatures_RealEstates_RealEstatesId",
                        column: x => x.RealEstatesId,
                        principalTable: "RealEstates",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RealEstates_SpecialFeatures_SpecialFeature_SpecialFeatureId",
                        column: x => x.SpecialFeatureId,
                        principalTable: "SpecialFeature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookMarks_RealEstatesId",
                table: "BookMarks",
                column: "RealEstatesId");

            migrationBuilder.CreateIndex(
                name: "IX_RealEstates_Facilities_FacilitiesId",
                table: "RealEstates_Facilities",
                column: "FacilitiesId");

            migrationBuilder.CreateIndex(
                name: "IX_RealEstates_Facilities_RealEstatesId",
                table: "RealEstates_Facilities",
                column: "RealEstatesId");

            migrationBuilder.CreateIndex(
                name: "IX_RealEstates_SpecialFeatures_RealEstatesId",
                table: "RealEstates_SpecialFeatures",
                column: "RealEstatesId");

            migrationBuilder.CreateIndex(
                name: "IX_RealEstates_SpecialFeatures_SpecialFeatureId",
                table: "RealEstates_SpecialFeatures",
                column: "SpecialFeatureId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookMarks");

            migrationBuilder.DropTable(
                name: "RealEstates_Facilities");

            migrationBuilder.DropTable(
                name: "RealEstates_SpecialFeatures");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "RealEstates");
        }
    }
}
