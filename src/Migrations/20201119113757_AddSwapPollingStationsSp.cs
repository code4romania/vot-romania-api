using Microsoft.EntityFrameworkCore.Migrations;

namespace VotRomania.Migrations
{
    public partial class AddSwapPollingStationsSp : Migration
    {
        private readonly string spCreate = @"
		CREATE OR REPLACE PROCEDURE public.swappollingstations()
		LANGUAGE 'plpgsql'
		AS $BODY$
			begin
				DELETE FROM public.""PollingStations"";
				ALTER TABLE public.""PollingStations"" ADD column ""IpsId"" INT;
			
				INSERT INTO public.""PollingStations""(
					""IpsId"",
					""Latitude"", 
					""Longitude"", 
					""County"", 
					""Locality"", 
					""PollingStationNumber"", 
					""Institution"", 
					""Address""
				)
				  SELECT ""Id"", ""Latitude"", ""Longitude"", ""County"", ""Locality"", ""PollingStationNumber"", ""Institution"", ""Address""
				  FROM public.""ImportedPollingStations"";
				  
				  
				INSERT INTO public.""PollingStationsAddresses""(
					""Locality"", 
					""StreetCode"", 
					""Street"", 
					""HouseNumbers"", 
					""Remarks"", 
					""PollingStationId""
				)
				SELECT ipsa.""Locality"", ipsa.""StreetCode"", ipsa.""Street"", ipsa.""HouseNumbers"", ipsa.""Remarks"", ps.""Id""
				FROM public.""PollingStations"" AS ps
				INNER JOIN public.""ImportedPollingStationAddresses"" AS ipsa
				ON ipsa.""ImportedPollingStationId"" = ps.""IpsId"";
				
				ALTER TABLE public.""PollingStations"" DROP COLUMN ""IpsId"";
				
				DELETE FROM public.""ImportedPollingStations""
				commit;
			end;$BODY$;
            ";

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(spCreate);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("drop procedure if exists swappollingstations ");
        }
    }
}
