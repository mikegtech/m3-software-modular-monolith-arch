using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace M3.Net.Modules.Transcripts.Infrastructure.Database;

/// <summary>
/// Design-time factory for creating TranscriptsDbContext instances during migrations
/// </summary>
public sealed class TranscriptsDbContextFactory : IDesignTimeDbContextFactory<TranscriptsDbContext>
{
    public TranscriptsDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("Database")
            ?? "Host=localhost;Database=m3net;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<TranscriptsDbContext>();
        optionsBuilder.UseNpgsql(connectionString, options =>
        {
            options.MigrationsHistoryTable("__EFMigrationsHistory", Schemas.Transcripts);
        });

        return new TranscriptsDbContext(optionsBuilder.Options);
    }
}
