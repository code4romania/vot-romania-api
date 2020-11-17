using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VotRomania.Stores.Entities;

namespace VotRomania.Stores
{
    public class VotRomaniaContext : DbContext
    {
        public VotRomaniaContext(DbContextOptions<VotRomaniaContext> options) : base(options) { }

        public DbSet<ApplicationContentEntity> ApplicationContent { get; set; }
        public DbSet<PollingStationEntity> PollingStations { get; set; }
        public DbSet<PollingStationAddressEntity> PollingStationAddresses { get; set; }
        public DbSet<UploadJobsEntity> ImportJobs { get; set; }
        public DbSet<ImportedPollingStationEntity> ImportedPollingStations { get; set; }
        public DbSet<ImportedPollingStationAddressEntity> ImportedPollingStationAddresses { get; set; }
        public DbSet<ResolvedAddressEntity> ResolvedAddresses { get; set; }

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
                    .HasColumnName("Locality");

                entity.Property(m => m.StreetCode)
                    .HasColumnName("StreetCode");

                entity.Property(m => m.Street)
                    .HasColumnName("Street");

                entity.Property(m => m.HouseNumbers)
                    .HasColumnName("HouseNumbers");

                entity.Property(m => m.Remarks)
                    .HasColumnName("Remarks");

                entity.HasOne(d => d.PollingStation)
                    .WithMany(p => p.PollingStationAddresses)
                    .HasForeignKey(d => d.PollingStationId)
                    .OnDelete(DeleteBehavior.Cascade);
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

            builder.Entity<UploadJobsEntity>(entity =>
            {
                entity.ToTable("ImportJobs");

                entity.Property(m => m.Id).IsRequired();
                entity.HasKey(m => m.Id);
                entity.HasIndex(m => m.Id);

                entity.Property(m => m.JobId)
                    .HasColumnName("JobId")
                    .IsRequired();

                entity.Property(m => m.FileName)
                    .HasColumnName("FileName")
                    .IsRequired();

                entity.Property(m => m.Base64File)
                    .HasColumnName("Base64File")
                    .IsRequired();

                entity.Property(m => m.JobStatus)
                    .HasColumnName("JobStatus")
                    .IsRequired();

                entity.Property(m => m.Started)
                    .HasColumnName("Started");

                entity.Property(m => m.Ended)
                    .HasColumnName("Ended");
            });

            builder.Entity<ImportedPollingStationEntity>(entity =>
            {
                entity.ToTable("ImportedPollingStations");
                entity.Property(m => m.Id).IsRequired();
                entity.HasKey(m => m.Id);
                entity.HasIndex(m => m.Id);

                entity.Property(m => m.JobId)
                    .HasColumnName("JobId")
                    .IsRequired();

                entity.Property(m => m.Longitude)
                    .HasColumnName("Longitude")
                    .HasDefaultValue();

                entity.Property(m => m.Latitude)
                    .HasColumnName("Latitude")
                    .HasDefaultValue();


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

                entity.Property(m => m.ResolvedAddressStatus)
                    .HasColumnName("ResolvedAddressStatus")
                    .IsRequired();

                entity.Property(m => m.FailMessage)
                    .HasColumnName("FailMessage");

                entity.HasMany(x => x.AssignedAddresses);
            });


            builder.Entity<ImportedPollingStationAddressEntity>(entity =>
            {
                entity.ToTable("ImportedPollingStationAddresses");
                entity.Property(m => m.Id).IsRequired();
                entity.HasKey(m => m.Id);
                entity.HasIndex(m => m.Id);

                entity.Property(m => m.Locality)
                    .HasColumnName("Locality");
                    
                entity.Property(m => m.StreetCode)
                    .HasColumnName("StreetCode");

                entity.Property(m => m.Street)
                    .HasColumnName("Street");

                entity.Property(m => m.HouseNumbers)
                    .HasColumnName("HouseNumbers");

                entity.Property(m => m.Remarks)
                    .HasColumnName("Remarks");
                    
                entity.HasOne(d => d.ImportedPollingStation)
                    .WithMany(p => p.AssignedAddresses)
                    .HasForeignKey(d => d.ImportedPollingStationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<ResolvedAddressEntity>(entity =>
            {
                entity.ToTable("ResolvedAddresses");
                entity.Property(m => m.Id).IsRequired();
                entity.HasKey(m => m.Id);
                entity.HasIndex(r => new { r.County, r.Locality, r.Address });

                entity.HasIndex(m => m.Id);

                entity.Property(m => m.County)
                    .HasColumnName("County");

                entity.Property(m => m.Locality)
                    .HasColumnName("Locality");

                entity.Property(m => m.Address)
                    .HasColumnName("Address");

                entity.Property(m => m.Latitude)
                    .HasColumnName("Latitude");

                entity.Property(m => m.Longitude)
                    .HasColumnName("Longitude");

            });

            base.OnModelCreating(builder);
        }

    }
}