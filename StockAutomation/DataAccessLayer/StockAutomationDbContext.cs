using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataAccessLayer;

public class StockAutomationDbContext : DbContext
{
    public DbSet<Configuration> Configurations { get; set; }
    public DbSet<Snapshot> Snapshots { get; set; }
    public DbSet<Subscriber> Subscribers { get; set; }

    public StockAutomationDbContext(DbContextOptions options) : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseLoggerFactory(LoggerFactory.Create(
                builder =>
                {
                    builder.AddFilter((category, level) => category == DbLoggerCategory.Database.Command.Name
                                                           && level == LogLevel.Information);
                })).EnableSensitiveDataLogging();
    }

    // https://docs.microsoft.com/en-us/ef/core/modeling/
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Cascade;
        }
        base.OnModelCreating(modelBuilder);
    }
}
