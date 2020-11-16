using Microsoft.EntityFrameworkCore.Migrations;

namespace VotRomania.Migrations
{
    public partial class AddAddressBankInsertSP : Migration
    {
        private readonly string spCreate = @"
            create or replace procedure PopulateAddressBank()
            language plpgsql    
            as $$
            begin
                INSERT INTO public.""ResolvedAddresses""(""County"", ""Locality"", ""Address"", ""Latitude"", ""Longitude"")
                select ps.""County"", ps.""Locality"", ps.""Address"", ps.""Latitude"", ps.""Longitude""
                from public.""PollingStations"" as ps
                left join public.""ResolvedAddresses"" as rs
                on rs.""County"" = ps.""County"" 
                and rs.""Locality"" = ps.""Locality""
                and rs.""Address"" = ps.""Address""
                where rs.""County"" is null and rs.""Locality"" is null and rs.""Address"" is null;
                
                commit;
            end;$$
            ";
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(spCreate);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("drop procedure if exists PopulateAddressBank ");
        }
    }
}
