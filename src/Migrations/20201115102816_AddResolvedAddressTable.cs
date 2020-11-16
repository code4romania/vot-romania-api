using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace VotRomania.Migrations
{
    public partial class AddResolvedAddressTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ResolvedAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    County = table.Column<string>(nullable: false),
                    Locality = table.Column<string>(nullable: false),
                    Address = table.Column<string>(nullable: false),
                    Latitude = table.Column<double>(nullable: false),
                    Longitude = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResolvedAddresses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResolvedAddresses_Id",
                table: "ResolvedAddresses",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ResolvedAddresses_County_Locality_Address",
                table: "ResolvedAddresses",
                columns: new[] { "County", "Locality", "Address" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResolvedAddresses");
        }
    }
}
