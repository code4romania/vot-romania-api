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
                name: "PollingStationsAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Locality = table.Column<string>(nullable: false),
                    StreetCode = table.Column<string>(nullable: false),
                    Street = table.Column<string>(nullable: false),
                    HouseNumbers = table.Column<string>(nullable: false),
                    Remarks = table.Column<string>(nullable: true),
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
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationContent_Language",
                table: "ApplicationContent",
                column: "Language");

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
                name: "PollingStationsAddresses");

            migrationBuilder.DropTable(
                name: "PollingStations");
        }
    }
}
