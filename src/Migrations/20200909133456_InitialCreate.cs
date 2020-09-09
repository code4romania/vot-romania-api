using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace VotRomania.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationContent",
                columns: table => new
                {
                    Language = table.Column<int>(nullable: false),
                    Data = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationContent", x => x.Language);
                });

            migrationBuilder.CreateTable(
                name: "ImportedPollingStations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Latitude = table.Column<double>(nullable: true),
                    Longitude = table.Column<double>(nullable: true),
                    County = table.Column<string>(nullable: false),
                    Locality = table.Column<string>(nullable: false),
                    PollingStationNumber = table.Column<string>(nullable: false),
                    Institution = table.Column<string>(nullable: false),
                    Address = table.Column<string>(nullable: false),
                    JobId = table.Column<string>(nullable: false),
                    ResolvedAddressStatus = table.Column<int>(nullable: false),
                    FailMessage = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportedPollingStations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImportJobs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobId = table.Column<string>(nullable: false),
                    FileName = table.Column<string>(nullable: false),
                    Base64File = table.Column<string>(nullable: false),
                    JobStatus = table.Column<int>(nullable: false),
                    Started = table.Column<DateTime>(nullable: true),
                    Ended = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PollingStations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Latitude = table.Column<double>(nullable: false),
                    Longitude = table.Column<double>(nullable: false),
                    County = table.Column<string>(nullable: false),
                    Locality = table.Column<string>(nullable: false),
                    PollingStationNumber = table.Column<string>(nullable: false),
                    Institution = table.Column<string>(nullable: false),
                    Address = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollingStations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImportedPollingStationAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Locality = table.Column<string>(nullable: true),
                    StreetCode = table.Column<string>(nullable: true),
                    Street = table.Column<string>(nullable: true),
                    HouseNumbers = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    ImportedPollingStationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportedPollingStationAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImportedPollingStationAddresses_ImportedPollingStations_Imp~",
                        column: x => x.ImportedPollingStationId,
                        principalTable: "ImportedPollingStations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PollingStationsAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Locality = table.Column<string>(nullable: false),
                    StreetCode = table.Column<string>(nullable: false),
                    Street = table.Column<string>(nullable: false),
                    HouseNumbers = table.Column<string>(nullable: false),
                    Remarks = table.Column<string>(nullable: false),
                    PollingStationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollingStationsAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PollingStationsAddresses_PollingStations_PollingStationId",
                        column: x => x.PollingStationId,
                        principalTable: "PollingStations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationContent_Language",
                table: "ApplicationContent",
                column: "Language");

            migrationBuilder.CreateIndex(
                name: "IX_ImportedPollingStationAddresses_Id",
                table: "ImportedPollingStationAddresses",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ImportedPollingStationAddresses_ImportedPollingStationId",
                table: "ImportedPollingStationAddresses",
                column: "ImportedPollingStationId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportedPollingStations_Id",
                table: "ImportedPollingStations",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ImportJobs_Id",
                table: "ImportJobs",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PollingStations_Id",
                table: "PollingStations",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationsAddresses_Id",
                table: "PollingStationsAddresses",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationsAddresses_PollingStationId",
                table: "PollingStationsAddresses",
                column: "PollingStationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationContent");

            migrationBuilder.DropTable(
                name: "ImportedPollingStationAddresses");

            migrationBuilder.DropTable(
                name: "ImportJobs");

            migrationBuilder.DropTable(
                name: "PollingStationsAddresses");

            migrationBuilder.DropTable(
                name: "ImportedPollingStations");

            migrationBuilder.DropTable(
                name: "PollingStations");
        }
    }
}
