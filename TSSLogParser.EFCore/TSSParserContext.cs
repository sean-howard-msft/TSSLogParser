using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace TSSLogParser.EFCore
{
    public partial class TSSParserContext : DbContext
    {
        private string connectionString;

        public TSSParserContext()
        {

        }

        public TSSParserContext(string ConnectionString)
        {
            connectionString = ConnectionString;
        }

        public TSSParserContext(DbContextOptions<TSSParserContext> options)
            : base(options)
        {

        }

        public virtual DbSet<EventLog> EventLogs { get; set; }
        public virtual DbSet<EventLogsClean> EventLogsCleans { get; set; }
        public virtual DbSet<GlobalMachineCount> GlobalMachineCounts { get; set; }
        public virtual DbSet<GlobalMessageCount> GlobalMessageCounts { get; set; }
        public virtual DbSet<GlobalTotal> GlobalTotals { get; set; }
        public virtual DbSet<MachineMetadatum> MachineMetadata { get; set; }
        public virtual DbSet<RegionalMachineCount> RegionalMachineCounts { get; set; }
        public virtual DbSet<RegionalMessageCount> RegionalMessageCounts { get; set; }
        public virtual DbSet<RegionalTotal> RegionalTotals { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(connectionString, options => options.CommandTimeout(600));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<EventLog>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.MachineName, e.LogName });

                entity.HasIndex(e => new { e.LogName, e.ProviderName }, "idx_EventLogs_LogName_ProviderName_i_Message");

                entity.Property(e => e.MachineName).HasMaxLength(50);

                entity.Property(e => e.LogName).HasMaxLength(100);

                entity.Property(e => e.ContainerLog)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.LevelDisplayName).HasMaxLength(255);

                entity.Property(e => e.ProviderName).HasMaxLength(255);
            });

            modelBuilder.Entity<EventLogsClean>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("EventLogsClean");

                entity.Property(e => e.ContainerLog)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.LevelDisplayName).HasMaxLength(255);

                entity.Property(e => e.LogName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.MachineName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ProviderName).HasMaxLength(255);
            });

            modelBuilder.Entity<GlobalMachineCount>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("GlobalMachineCounts");

                entity.Property(e => e.LogName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.ProviderName).HasMaxLength(255);
            });

            modelBuilder.Entity<GlobalMessageCount>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("GlobalMessageCounts");

                entity.Property(e => e.LogName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.ProviderName).HasMaxLength(255);
            });

            modelBuilder.Entity<GlobalTotal>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("GlobalTotals");

                entity.Property(e => e.LogName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.ProviderName).HasMaxLength(255);
            });

            modelBuilder.Entity<MachineMetadatum>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("MachineMetadata");

                entity.Property(e => e.AppCode).HasMaxLength(3);

                entity.Property(e => e.CountryCode).HasMaxLength(2);

                entity.Property(e => e.Domain).HasMaxLength(50);

                entity.Property(e => e.InfraCode).HasMaxLength(2);

                entity.Property(e => e.InstanceNum).HasMaxLength(2);

                entity.Property(e => e.MachineName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.MachineType).HasMaxLength(3);

                entity.Property(e => e.Region).HasMaxLength(3);
            });

            modelBuilder.Entity<RegionalMachineCount>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("RegionalMachineCounts");

                entity.Property(e => e.CountryCode).HasMaxLength(2);

                entity.Property(e => e.LogName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.ProviderName).HasMaxLength(255);

                entity.Property(e => e.Region).HasMaxLength(3);
            });

            modelBuilder.Entity<RegionalMessageCount>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("RegionalMessageCounts");

                entity.Property(e => e.AppCode).HasMaxLength(3);

                entity.Property(e => e.CountryCode).HasMaxLength(2);

                entity.Property(e => e.Domain).HasMaxLength(50);

                entity.Property(e => e.InfraCode).HasMaxLength(2);

                entity.Property(e => e.InstanceNum).HasMaxLength(2);

                entity.Property(e => e.LogName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.MachineName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.MachineType).HasMaxLength(3);

                entity.Property(e => e.ProviderName).HasMaxLength(255);

                entity.Property(e => e.Region).HasMaxLength(3);
            });

            modelBuilder.Entity<RegionalTotal>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("RegionalTotals");

                entity.Property(e => e.CountryCode).HasMaxLength(2);

                entity.Property(e => e.LogName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.ProviderName).HasMaxLength(255);

                entity.Property(e => e.Region).HasMaxLength(3);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
