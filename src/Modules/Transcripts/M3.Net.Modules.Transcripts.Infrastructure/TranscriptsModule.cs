using M3.Net.Common.Application.Authorization;
using M3.Net.Common.Application.EventBus;
using M3.Net.Common.Application.Messaging;
using M3.Net.Common.Infrastructure.Outbox;
using M3.Net.Common.Presentation.Endpoints;
using M3.Net.Modules.Transcripts.Application.Abstractions.Data;
using M3.Net.Modules.Transcripts.Domain.Transcripts;
using M3.Net.Modules.Transcripts.Infrastructure.Database;
using M3.Net.Modules.Transcripts.Infrastructure.Inbox;
using M3.Net.Modules.Transcripts.Infrastructure.Outbox;
using M3.Net.Modules.Transcripts.Infrastructure.ProcessingJobs;
using M3.Net.Modules.Transcripts.Infrastructure.TranscriptDeliveries;
using M3.Net.Modules.Transcripts.Infrastructure.TranscriptRequests;
using M3.Net.Modules.Transcripts.Infrastructure.Transcripts;
using M3.Net.Modules.Transcripts.IntegrationEvents;
using M3.Net.Modules.Transcripts.Presentation.TranscriptProcessing;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace M3.Net.Modules.Transcripts.Infrastructure;

public static class TranscriptsModule
{
    public static IServiceCollection AddTranscriptsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDomainEventHandlers();

        services.AddIntegrationEventHandlers();

        services.AddInfrastructure(configuration);

        services.AddEndpoints(Presentation.AssemblyReference.Assembly);

        return services;
    }

    private static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TranscriptsDbContext>((sp, options) =>
            options
                .UseNpgsql(
                    configuration.GetConnectionString("Database"),
                    npgsqlOptions => npgsqlOptions
                        .MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Transcripts))
                .AddInterceptors(sp.GetRequiredService<InsertOutboxMessagesInterceptor>())
                .UseSnakeCaseNamingConvention());

        services.AddScoped<ITranscriptRequestRepository, TranscriptRequestRepository>();
        services.AddScoped<IProcessingJobRepository, ProcessingJobRepository>();
        services.AddScoped<ITranscriptRepository, TranscriptRepository>();
        services.AddScoped<ITranscriptDeliveryRepository, TranscriptDeliveryRepository>();

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<TranscriptsDbContext>());

        services.Configure<OutboxOptions>(configuration.GetSection("Transcripts:Outbox"));

        services.ConfigureOptions<ConfigureProcessOutboxJob>();

        services.Configure<InboxOptions>(configuration.GetSection("Transcripts:Inbox"));

        services.ConfigureOptions<ConfigureProcessInboxJob>();
    }

    private static void AddDomainEventHandlers(this IServiceCollection services)
    {
        Type[] domainEventHandlers = Application.AssemblyReference.Assembly
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IDomainEventHandler)))
            .ToArray();

        foreach (Type domainEventHandler in domainEventHandlers)
        {
            services.TryAddScoped(domainEventHandler);

            Type domainEvent = domainEventHandler
                .GetInterfaces()
                .Single(i => i.IsGenericType)
                .GetGenericArguments()
                .Single();

            Type closedIdempotentHandler = typeof(IdempotentDomainEventHandler<>).MakeGenericType(domainEvent);

            services.Decorate(domainEventHandler, closedIdempotentHandler);
        }
    }

    private static void AddIntegrationEventHandlers(this IServiceCollection services)
    {
        Type[] integrationEventHandlers = Presentation.AssemblyReference.Assembly
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IIntegrationEventHandler)))
            .ToArray();

        foreach (Type integrationEventHandler in integrationEventHandlers)
        {
            services.TryAddScoped(integrationEventHandler);

            Type integrationEvent = integrationEventHandler
                .GetInterfaces()
                .Single(i => i.IsGenericType)
                .GetGenericArguments()
                .Single();

            Type closedIdempotentHandler =
                typeof(IdempotentIntegrationEventHandler<>).MakeGenericType(integrationEvent);

            services.Decorate(integrationEventHandler, closedIdempotentHandler);
        }
    }
}
