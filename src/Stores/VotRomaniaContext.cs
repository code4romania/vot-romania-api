using System.IO;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VotRomania.Options;
using VotRomania.Stores.Entities;

namespace VotRomania.Stores
{
    public class VotRomaniaContext : DbContext
    {
        private readonly ILogger _logger;
        private readonly DatabaseOptions _databaseOptions;

        public VotRomaniaContext(
            ILogger<VotRomaniaContext> logger,
            IOptions<DatabaseOptions> databaseOptions)
        {
            _logger = logger;
            _databaseOptions = databaseOptions.Value;
        }

        public DbSet<PollingStationEntity> PollingStations { get; set; }
        public DbSet<ApplicationContentEntity> ApplicationContent { get; set; }
        public DbSet<PollingStationAddressEntity> PollingStationAddresses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = _databaseOptions.DatabasePath.Replace("\\", Path.DirectorySeparatorChar.ToString());
            connectionStringBuilder.Mode = SqliteOpenMode.ReadWriteCreate;
            optionsBuilder.UseSqlite(connectionStringBuilder.ConnectionString);

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<PollingStationEntity>(entity =>
            {
                entity.ToTable("PollingStations");
                entity.Property(m => m.Id).IsRequired();
                entity.HasKey(m => m.Id);
                entity.HasIndex(m => m.Id);

                entity.Property(m => m.Longitude)
                    .HasColumnName("Longitude")
                    .IsRequired();

                entity.Property(m => m.Latitude)
                    .HasColumnName("Latitude")
                    .IsRequired();

                entity.Property(m => m.Address)
                    .HasColumnName("Address")
                    .IsRequired();

                entity.Property(m => m.County)
                    .HasColumnName("County")
                    .IsRequired();

                entity.Property(m => m.Institution)
                    .HasColumnName("Institution")
                    .IsRequired();

                entity.Property(m => m.Locality)
                    .HasColumnName("Locality")
                    .IsRequired();

                entity.Property(m => m.PollingStationNumber)
                    .HasColumnName("PollingStationNumber")
                    .IsRequired();


                entity.HasMany(x => x.PollingStationAddresses);
            });

            builder.Entity<PollingStationAddressEntity>(entity =>
            {
                entity.ToTable("PollingStationsAddresses");
                entity.Property(m => m.Id).IsRequired();
                entity.HasKey(m => m.Id);
                entity.HasIndex(m => m.Id);

                entity.Property(m => m.Locality)
                      .HasColumnName("Locality")
                      .IsRequired();

                entity.Property(m => m.StreetCode)
                    .HasColumnName("StreetCode")
                    .IsRequired();

                entity.Property(m => m.Street)
                    .HasColumnName("Street")
                    .IsRequired();

                entity.Property(m => m.HouseNumbers)
                    .HasColumnName("HouseNumbers")
                    .IsRequired();

                entity.Property(m => m.Remarks)
                    .HasColumnName("Remarks");

                entity.HasOne(d => d.PollingStation)
                    .WithMany(p => p.PollingStationAddresses)
                    .HasForeignKey(d => d.PollingStationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });


            builder.Entity<ApplicationContentEntity>(entity =>
            {
                entity.ToTable("ApplicationContent");

                entity.Property(m => m.Language).IsRequired();
                entity.HasKey(m => m.Language);
                entity.HasIndex(m => m.Language);

                entity.Property(m => m.Data)
                    .HasColumnName("Data")
                    .IsRequired();

            });

            base.OnModelCreating(builder);
        }

    }
}