using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

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
        public virtual DbSet<GlobalCount> GlobalCounts { get; set; }
        public virtual DbSet<MachineCount> MachineCounts { get; set; }
        public virtual DbSet<MachineMetadatum> MachineMetadata { get; set; }
        public virtual DbSet<MessageCount> MessageCounts { get; set; }
        public virtual DbSet<RegionalCount> RegionalCounts { get; set; }

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

            modelBuilder.Entity<GlobalCount>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("GlobalCounts");

                entity.Property(e => e.LogName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.ProviderName).HasMaxLength(255);
            });

            modelBuilder.Entity<MachineCount>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("MachineCount");

                entity.Property(e => e.LogName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.MachineCount1).HasColumnName("MachineCount");

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

            modelBuilder.Entity<MessageCount>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("MessageCount");

                entity.Property(e => e.LogName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.MachineName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.MessageCount1).HasColumnName("MessageCount");

                entity.Property(e => e.ProviderName).HasMaxLength(255);
            });

            modelBuilder.Entity<RegionalCount>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("RegionalCounts");

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

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
