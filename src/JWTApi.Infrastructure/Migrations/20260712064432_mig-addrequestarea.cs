using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JWTApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class migaddrequestarea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "RealEstatesApplicants_Sequence");

            migrationBuilder.CreateTable(
                name: "RealEstatesApplicants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<int>(type: "int", nullable: false, defaultValueSql: "NEXT VALUE FOR RealEstatesApplicants_Sequence"),
                    MinCountRoom = table.Column<int>(type: "int", nullable: true),
                    MinConstructionYear = table.Column<int>(type: "int", nullable: true),
                    MaxConstructionYear = table.Column<int>(type: "int", nullable: true),
                    MinSquareMeter = table.Column<int>(type: "int", nullable: true),
                    MaxSquareMeter = table.Column<int>(type: "int", nullable: true),
                    RegionId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Desc = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "0")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RealEstatesApplicants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RealEstatesApplicants_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RealEstatesApplicants_Regions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RealEstatesApplicants_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RealEstatesApplicants_CategoryId",
                table: "RealEstatesApplicants",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RealEstatesApplicants_Main",
                table: "RealEstatesApplicants",
                columns: new[] { "IsDeleted", "CategoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_RealEstatesApplicants_Region",
                table: "RealEstatesApplicants",
                columns: new[] { "RegionId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_RealEstatesApplicants_SerialNumber",
                table: "RealEstatesApplicants",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RealEstatesApplicants_User",
                table: "RealEstatesApplicants",
                columns: new[] { "UserId", "IsDeleted" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RealEstatesApplicants");

            migrationBuilder.DropSequence(
                name: "RealEstatesApplicants_Sequence");
        }
    }
}
