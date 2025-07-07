# M3.Net to Python Integration - Step-by-Step Implementation Guide

## Overview

This guide provides the step-by-step process to integrate the .NET M3 API with the Python transcript processing microservices via RabbitMQ message broker.

## Architecture Flow

```
┌─────────────────────┐    ┌─────────────────────┐    ┌─────────────────────┐    ┌─────────────────────┐
│   .NET M3 API       │    │  transcript-        │    │  transcript-api     │    │  transcript-mcp     │
│                     │    │  consumer           │    │  (AI Agent)         │    │  (MCP Server)       │
├─────────────────────┤    ├─────────────────────┤    ├─────────────────────┤    ├─────────────────────┤
│ • Request Transcript│───▶│ • Consume Events    │───▶│ • Process Video     │───▶│ • Video Tools       │
│ • Publish Events    │    │ • Route to API      │    │ • Generate Content  │    │ • Clip Generation   │
│ • Receive Progress  │◄───┤ • Publish Progress  │◄───┤ • Return Results    │    │ • Search & Analysis │
│ • Handle Completion │    │ • Handle Errors     │    │ • Error Handling    │    │ • PixelTable DB     │
└─────────────────────┘    └─────────────────────┘    └─────────────────────┘    └─────────────────────┘
         │                           │                           │                           │
         └───────────────────────────▼───────────────────────────▼───────────────────────────┘
                                RabbitMQ Message Broker
```

## Message Flow

1. **User Request**: User requests transcript via .NET API
2. **Event Publishing**: .NET publishes `TranscriptRequestedIntegrationEvent`
3. **Event Consumption**: transcript-consumer receives and processes event
4. **API Routing**: consumer routes request to transcript-api
5. **Video Processing**: transcript-api processes video using MCP tools
6. **Progress Updates**: progress events published back to .NET
7. **Completion**: final results published back to .NET

## Prerequisites

- .NET 8+ SDK
- Python 3.12+
- RabbitMQ server
- Docker (optional, for containerized deployment)
- Groq API key for AI processing

## Step 1: .NET Integration Event Setup

### 1.1 Integration Events (✅ COMPLETED)

The following integration events have been created:

- `TranscriptRequestedIntegrationEvent.cs` - Published by .NET when transcript is requested
- `TranscriptProcessingProgressIntegrationEvent.cs` - Consumed by .NET for progress updates
- `TranscriptProcessingCompletedIntegrationEvent.cs` - Consumed by .NET for final results

### 1.2 Event Handlers (✅ COMPLETED)

Created handlers for incoming events from Python:

- `TranscriptProcessingProgressIntegrationEventHandler.cs`
- `TranscriptProcessingCompletedIntegrationEventHandler.cs`

### 1.3 MassTransit Configuration (✅ COMPLETED)

Updated `Program.cs` to include consumer configuration:
```csharp
builder.Services.AddInfrastructure(
    DiagnosticsConfig.ServiceName,
    [M3.Net.Modules.Transcripts.Infrastructure.TranscriptsModule.ConfigureConsumers],
    rabbitMqSettings,
    databaseConnectionString,
    redisConnectionString);
```

## Step 2: Python Consumer Service Setup

### 2.1 Updated transcript-consumer (✅ COMPLETED)

Key updates made:

1. **Message Contract Validation**: Added Pydantic models matching .NET events
2. **HTTP Client**: Added httpx for communicating with transcript-api
3. **Progress Publishing**: Publishes progress events back to .NET
4. **Error Handling**: Comprehensive error handling and retry logic
5. **State Persistence**: Maintains state of processed requests

### 2.2 New Dependencies (✅ COMPLETED)

Added to `pyproject.toml`:
- `httpx>=0.28.1` - HTTP client for API calls
- `pydantic>=2.10.5` - Data validation and message contracts

### 2.3 Environment Configuration (✅ COMPLETED)

Required environment variables:
- `RABBITMQ_URL` - RabbitMQ connection string
- `TRANSCRIPT_API_URL` - URL for transcript-api service

## Step 3: Python API Service Setup

### 3.1 New Endpoint (✅ COMPLETED)

Added `/process-video-from-consumer` endpoint to handle requests from consumer:

```python
@app.post("/process-video-from-consumer", response_model=ProcessVideoFromConsumerResponse)
async def process_video_from_consumer(request: ProcessVideoFromConsumerRequest, fastapi_request: Request):
```

### 3.2 Request/Response Models (✅ COMPLETED)

Added new Pydantic models:
- `ProcessVideoFromConsumerRequest` - Incoming requests from consumer
- `ProcessVideoFromConsumerResponse` - Responses back to consumer

## Step 4: RabbitMQ Exchange Configuration

### 4.1 Exchange Naming Convention

MassTransit uses specific naming conventions for exchanges:

**Incoming (from .NET):**
- `m3-net-modules-transcripts-integration-events:transcript-requested-integration-event`

**Outgoing (to .NET):**
- `m3-net-modules-transcripts-integration-events:transcript-processing-progress-integration-event`
- `m3-net-modules-transcripts-integration-events:transcript-processing-completed-integration-event`

### 4.2 Queue Configuration

- Queue: `transcript-consumer`
- Type: Quorum queue for high availability
- Durable: Yes, survives broker restarts

## Step 5: Deployment Configuration

### 5.1 Environment Variables

Create `.env` file for Python services (see `.env.example`):

```bash
# Required
RABBITMQ_URL=amqp://guest:guest@rabbitmq:5672/
TRANSCRIPT_API_URL=http://transcript-api:8080
GROQ_API_KEY=your_groq_api_key_here

# Optional
OPIK_API_KEY=your_opik_api_key_here
MCP_SERVER=http://transcript-mcp:9090/mcp
```

### 5.2 Docker Compose Updates (PENDING)

Update `docker-compose.yml` to include:

1. **transcript-consumer service**
2. **transcript-api service** 
3. **transcript-mcp service**
4. **Shared volume** for media files
5. **Network configuration** for service communication

## Step 6: Implementation Steps (NEXT)

### 6.1 Install Python Dependencies

```bash
# For transcript-consumer
cd src/python/transcript-consumer
uv sync

# For transcript-api  
cd ../transcript-api
uv sync

# For transcript-mcp
cd ../transcript-mcp
uv sync
```

### 6.2 Update Database Schema (PENDING)

Add fields to track processing status:

```sql
ALTER TABLE transcript_requests 
ADD COLUMN processing_status VARCHAR(50) DEFAULT 'pending',
ADD COLUMN progress_percentage INTEGER DEFAULT 0,
ADD COLUMN error_message TEXT,
ADD COLUMN transcript_content TEXT,
ADD COLUMN processed_at TIMESTAMP;
```

### 6.3 Update .NET Event Handlers (PENDING)

Implement the TODO items in the integration event handlers:

1. **Update database status** when progress/completion events are received
2. **Store transcript content** when processing completes
3. **Handle error cases** and update request status accordingly
4. **Add SignalR notifications** for real-time UI updates (optional)

### 6.4 Test Integration Flow (PENDING)

1. **Start RabbitMQ** server
2. **Start Python services** (consumer, api, mcp)
3. **Start .NET API**
4. **Submit transcript request** via .NET API
5. **Verify message flow** through all components
6. **Check database updates** and final results

## Step 7: Error Handling & Resilience

### 7.1 Retry Policies

- **HTTP calls**: Retry with exponential backoff
- **RabbitMQ publishing**: Built-in retry with MassTransit
- **Database operations**: Transaction rollback on failure

### 7.2 Dead Letter Queues

Configure DLQ for:
- Failed transcript processing
- Invalid message formats
- Timeout scenarios

### 7.3 Health Checks

Add health check endpoints:
- `/health` for each Python service
- Database connectivity checks
- RabbitMQ connectivity checks
- External API availability checks

## Step 8: Monitoring & Observability

### 8.1 Logging

- **Structured logging** with correlation IDs
- **Request tracing** across service boundaries
- **Performance metrics** for processing times

### 8.2 Metrics

Track:
- Request processing times
- Success/failure rates
- Queue depths
- Resource utilization

### 8.3 Alerts

Set up alerts for:
- High error rates
- Processing delays
- Service unavailability
- Queue backlog

## Step 9: Testing Strategy

### 9.1 Unit Tests

- Message serialization/deserialization
- Event handler logic
- HTTP client interactions
- Database operations

### 9.2 Integration Tests

- End-to-end message flow
- Service-to-service communication
- Error scenarios and recovery
- Performance under load

### 9.3 Load Testing

- Concurrent request processing
- RabbitMQ throughput limits
- Database performance
- Resource scaling requirements

## Next Steps Summary

1. **✅ COMPLETED**: Created all integration events and handlers
2. **✅ COMPLETED**: Updated transcript-consumer with message routing
3. **✅ COMPLETED**: Added new endpoint to transcript-api
4. **⏳ PENDING**: Update database schema for tracking
5. **⏳ PENDING**: Implement database updates in .NET handlers
6. **⏳ PENDING**: Update docker-compose.yml with Python services
7. **⏳ PENDING**: End-to-end testing and validation
8. **⏳ PENDING**: Production deployment configuration

## Files Modified/Created

### .NET Files
- `TranscriptRequestedIntegrationEvent.cs` (existing)
- `TranscriptProcessingProgressIntegrationEvent.cs` (new)
- `TranscriptProcessingCompletedIntegrationEvent.cs` (new)
- `TranscriptProcessingProgressIntegrationEventHandler.cs` (new)
- `TranscriptProcessingCompletedIntegrationEventHandler.cs` (new)
- `TranscriptsModule.cs` (updated)
- `Program.cs` (updated)

### Python Files
- `transcript-consumer/src/transcript_consumer/transcripts_consumer.py` (updated)
- `transcript-consumer/src/transcript_consumer/app.py` (updated)
- `transcript-consumer/pyproject.toml` (updated)
- `transcript-api/src/transcript_api/models.py` (updated)
- `transcript-api/src/transcript_api/api.py` (updated)
- `src/python/.env.example` (new)

### Documentation
- This implementation guide (new)

The integration framework is now ready for testing and deployment!
