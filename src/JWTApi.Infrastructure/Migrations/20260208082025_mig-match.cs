using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JWTApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class migmatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RealEstates_Facilities_RealEstates_RealEstatesId",
                table: "RealEstates_Facilities");

            migrationBuilder.DropForeignKey(
                name: "FK_RealEstates_SpecialFeatures_RealEstates_RealEstatesId",
                table: "RealEstates_SpecialFeatures");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RealEstates_SpecialFeatures",
                table: "RealEstates_SpecialFeatures");

            migrationBuilder.DropIndex(
                name: "IX_RealEstates_SpecialFeatures_RealEstatesId",
                table: "RealEstates_SpecialFeatures");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RealEstates_Facilities",
                table: "RealEstates_Facilities");

            migrationBuilder.DropIndex(
                name: "IX_RealEstates_Facilities_RealEstatesId",
                table: "RealEstates_Facilities");

            migrationBuilder.DropColumn(
                name: "RealStatesId",
                table: "RealEstates_SpecialFeatures");

            migrationBuilder.DropColumn(
                name: "RealStatesId",
                table: "RealEstates_Facilities");

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

            migrationBuilder.AlterColumn<int>(
                name: "RealEstatesId",
                table: "RealEstates_SpecialFeatures",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RealEstatesId",
                table: "RealEstates_Facilities",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RealEstatesRentId",
                table: "BookMarks",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RealEstates_SpecialFeatures",
                table: "RealEstates_SpecialFeatures",
                columns: new[] { "RealEstatesId", "SpecialFeatureId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_RealEstates_Facilities",
                table: "RealEstates_Facilities",
                columns: new[] { "RealEstatesId", "FacilitiesId" });

            migrationBuilder.CreateTable(
                name: "RealEstatesRent_Facilities",
                columns: table => new
                {
                    RealEstatesRentId = table.Column<int>(type: "int", nullable: false),
                    FacilitiesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RealEstatesRent_Facilities", x => new { x.RealEstatesRentId, x.FacilitiesId });
                });

            migrationBuilder.CreateTable(
                name: "RealEstatesRent_SpecialFeatures",
                columns: table => new
                {
                    RealEstatesRentId = table.Column<int>(type: "int", nullable: false),
                    SpecialFeatureId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RealEstatesRent_SpecialFeatures", x => new { x.RealEstatesRentId, x.SpecialFeatureId });
                });

            migrationBuilder.CreateTable(
                name: "RealEstatesRents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AdditionalInformation = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    DescriptionRows = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    RoomCount = table.Column<int>(type: "int", nullable: false),
                    Floor = table.Column<int>(type: "int", nullable: false),
                    CountFloor = table.Column<int>(type: "int", nullable: false),
                    CountInFloor = table.Column<int>(type: "int", nullable: false),
                    ConstructionYear = table.Column<int>(type: "int", nullable: false),
                    SquareMeter = table.Column<int>(type: "int", nullable: false),
                    Deposit = table.Column<long>(type: "bigint", nullable: true),
                    DepositString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rent = table.Column<long>(type: "bigint", nullable: true),
                    RentString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(10,8)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(11,8)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    IsPrivate = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "0"),
                    RegionId = table.Column<int>(type: "int", nullable: false),
                    Convertible = table.Column<bool>(type: "bit", nullable: false),
                    IsShowLocation = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "0"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "0")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RealEstatesRents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RealEstatesRents_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RealEstatesRents_Regions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RealEstatesRents_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SearchRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MinSquareMeter = table.Column<int>(type: "int", nullable: true),
                    MaxSquareMeter = table.Column<int>(type: "int", nullable: true),
                    MinPrice = table.Column<int>(type: "int", nullable: true),
                    MaxPrice = table.Column<int>(type: "int", nullable: true),
                    MinFloor = table.Column<int>(type: "int", nullable: true),
                    MaxFloor = table.Column<int>(type: "int", nullable: true),
                    MinRoomCount = table.Column<int>(type: "int", nullable: true),
                    MaxRoomCount = table.Column<int>(type: "int", nullable: true),
                    LowUnitCount = table.Column<bool>(type: "bit", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    LastMatchDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SearchRequests_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SearchMatches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SearchRequestId = table.Column<int>(type: "int", nullable: false),
                    RealEstateId = table.Column<int>(type: "int", nullable: false),
                    RealEstateRentId = table.Column<int>(type: "int", nullable: false),
                    MatchScore = table.Column<double>(type: "float", nullable: false),
                    MatchReasons = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsNotified = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "0"),
                    IsSeenByUser = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "0"),
                    IsRejected = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "0"),
                    MatchedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    NotifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SeenAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchMatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SearchMatches_SearchRequests_SearchRequestId",
                        column: x => x.SearchRequestId,
                        principalTable: "SearchRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookMarks_RealEstatesRentId",
                table: "BookMarks",
                column: "RealEstatesRentId");

            migrationBuilder.CreateIndex(
                name: "IX_RealEstatesRents_CategoryId",
                table: "RealEstatesRents",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RealEstatesRents_RegionId",
                table: "RealEstatesRents",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_RealEstatesRents_UserId",
                table: "RealEstatesRents",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SearchMatches_SearchRequestId",
                table: "SearchMatches",
                column: "SearchRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_SearchRequests_CategoryId",
                table: "SearchRequests",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookMarks_RealEstatesRents_RealEstatesRentId",
                table: "BookMarks",
                column: "RealEstatesRentId",
                principalTable: "RealEstatesRents",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookMarks_RealEstatesRents_RealEstatesRentId",
                table: "BookMarks");

            migrationBuilder.DropTable(
                name: "RealEstatesRent_Facilities");

            migrationBuilder.DropTable(
                name: "RealEstatesRent_SpecialFeatures");

            migrationBuilder.DropTable(
                name: "RealEstatesRents");

            migrationBuilder.DropTable(
                name: "SearchMatches");

            migrationBuilder.DropTable(
                name: "SearchRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RealEstates_SpecialFeatures",
                table: "RealEstates_SpecialFeatures");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RealEstates_Facilities",
                table: "RealEstates_Facilities");

            migrationBuilder.DropIndex(
                name: "IX_BookMarks_RealEstatesRentId",
                table: "BookMarks");

            migrationBuilder.DropColumn(
                name: "RealEstatesRentId",
                table: "BookMarks");

            migrationBuilder.AlterColumn<int>(
                name: "RealEstatesId",
                table: "RealEstates_SpecialFeatures",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "RealStatesId",
                table: "RealEstates_SpecialFeatures",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "RealEstatesId",
                table: "RealEstates_Facilities",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "RealStatesId",
                table: "RealEstates_Facilities",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.AddPrimaryKey(
                name: "PK_RealEstates_SpecialFeatures",
                table: "RealEstates_SpecialFeatures",
                columns: new[] { "RealStatesId", "SpecialFeatureId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_RealEstates_Facilities",
                table: "RealEstates_Facilities",
                columns: new[] { "RealStatesId", "FacilitiesId" });

            migrationBuilder.CreateIndex(
                name: "IX_RealEstates_SpecialFeatures_RealEstatesId",
                table: "RealEstates_SpecialFeatures",
                column: "RealEstatesId");

            migrationBuilder.CreateIndex(
                name: "IX_RealEstates_Facilities_RealEstatesId",
                table: "RealEstates_Facilities",
                column: "RealEstatesId");

            migrationBuilder.AddForeignKey(
                name: "FK_RealEstates_Facilities_RealEstates_RealEstatesId",
                table: "RealEstates_Facilities",
                column: "RealEstatesId",
                principalTable: "RealEstates",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RealEstates_SpecialFeatures_RealEstates_RealEstatesId",
                table: "RealEstates_SpecialFeatures",
                column: "RealEstatesId",
                principalTable: "RealEstates",
                principalColumn: "Id");
        }
    }
}
