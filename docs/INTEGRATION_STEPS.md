# .NET to Python Integration Steps

This document outlines the comprehensive step-by-step process to integrate the .NET M3 API with the Python transcript processing services using RabbitMQ message broker.

## Overview

The integration follows a distributed, asynchronous architecture where:
1. **.NET API** publishes `TranscriptRequestedIntegrationEvent` to RabbitMQ
2. **transcript-consumer** consumes the event and routes to **transcript-api**
3. **transcript-api** processes the video using **transcript-mcp** tools
4. **transcript-consumer** publishes progress/completion events back to .NET
5. **.NET API** handles the completion events and updates the system

## Architecture Flow

```
┌─────────────────┐    ┌─────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   .NET M3 API   │───▶│  RabbitMQ   │───▶│transcript-consumer│───▶│ transcript-api  │
│                 │    │             │    │                 │    │                 │
│ TranscriptModule│◄───│  Exchanges  │◄───│   Python        │    │   Groq Agent    │
└─────────────────┘    └─────────────┘    └─────────────────┘    └─────────────────┘
                                                   │                        │
                                                   ▼                        ▼
                                          ┌─────────────────┐    ┌─────────────────┐
                                          │   RabbitMQ      │    │  transcript-mcp │
                                          │Progress/Complete│    │   MCP Server    │
                                          │    Events       │    │                 │
                                          └─────────────────┘    └─────────────────┘
```

## Changes Made

### 1. .NET Changes

#### A. Integration Events Created
- **TranscriptProcessingProgressIntegrationEvent.cs** - For progress updates
- **TranscriptProcessingCompletedIntegrationEvent.cs** - For completion/failure

#### B. Event Handlers Created
- **TranscriptProcessingProgressIntegrationEventHandler.cs** - Handles progress from Python
- **TranscriptProcessingCompletedIntegrationEventHandler.cs** - Handles completion from Python

#### C. Module Configuration Updated
- **TranscriptsModule.cs** - Added `ConfigureConsumers` method for MassTransit
- **Program.cs** - Added consumer configuration to infrastructure setup

#### D. Project References Updated
- **M3.Net.Modules.Transcripts.Presentation.csproj** - Added IntegrationEvents reference

### 2. Python Changes

#### A. transcript-consumer Service Updated
- **transcripts_consumer.py** - Complete rewrite to handle .NET message format
- **app.py** - Updated FastAPI endpoints for monitoring
- **pyproject.toml** - Added httpx and pydantic dependencies

#### B. transcript-api Service Updated
- **models.py** - Added new request/response models for consumer integration
- **api.py** - Added `/process-video-from-consumer` endpoint

#### C. Message Contracts Added
- **TranscriptRequestedIntegrationEvent** - Pydantic model matching .NET
- **TranscriptProcessingProgressEvent** - For publishing progress
- **TranscriptProcessingCompletedEvent** - For publishing completion

### 3. Environment Configuration
- **.env.example** - Created comprehensive environment variable template

## Step-by-Step Setup Instructions

### Prerequisites

1. **RabbitMQ Server** running (default: localhost:5672)
2. **PostgreSQL Database** for .NET application
3. **Python 3.12+** for Python services
4. **Required API Keys**:
   - Groq API key for transcript-api
   - Optional: Opik API key for observability

### Step 1: Set Up Python Environment

```bash
# Navigate to Python services directory
cd src/python

# Copy environment template
cp .env.example .env

# Edit .env file with your API keys
# Required: GROQ_API_KEY
# Optional: OPIK_API_KEY, PIXELTABLE_API_KEY, OPENAI_API_KEY
```

### Step 2: Install Python Dependencies

```bash
# transcript-consumer
cd transcript-consumer
uv sync  # or pip install -e .

# transcript-api
cd ../transcript-api
uv sync  # or pip install -e .

# transcript-mcp (if not already done)
cd ../transcript-mcp
uv sync  # or pip install -e .
```

### Step 3: Start Python Services

```bash
# Terminal 1: Start transcript-mcp server
cd src/python/transcript-mcp
uv run python src/transcript_mcp/server.py

# Terminal 2: Start transcript-api
cd src/python/transcript-api
uv run python src/transcript_api/api.py --port 8080

# Terminal 3: Start transcript-consumer
cd src/python/transcript-consumer
uv run python src/transcript_consumer/app.py
```

### Step 4: Start .NET Application

```bash
# Build the solution
dotnet build m3.net.sln

# Run the API
cd src/API
dotnet run
```

### Step 5: Test the Integration

#### A. Submit a Transcript Request

```bash
# POST to .NET API transcript endpoint
curl -X POST http://localhost:5000/api/transcripts/requests \
  -H "Content-Type: application/json" \
  -d '{
    "youTubeUrl": "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
    "title": "Test Video"
  }'
```

#### B. Monitor Progress

```bash
# Check transcript-consumer status
curl http://localhost:8000/info

# Check processed requests
curl http://localhost:8000/processed-requests

# Check transcript-api health
curl http://localhost:8080/
```

## Environment Variables Reference

### RabbitMQ Configuration (All Services)
```env
RABBITMQ_URL=amqp://guest:guest@localhost:5672/
```

### transcript-consumer Specific
```env
TRANSCRIPT_API_URL=http://localhost:8080
STATE_PATH=/app/data/state.json
```

### transcript-api Specific
```env
# Required
GROQ_API_KEY=your_groq_api_key_here

# Optional
MCP_SERVER=http://localhost:9090/mcp
OPIK_API_KEY=your_opik_api_key_here
AGENT_MEMORY_SIZE=20
```

### .NET API Configuration
Update `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "Database": "Host=localhost;Database=m3net;Username=postgres;Password=postgres",
    "Cache": "localhost:6379",
    "Queue": "amqp://guest:guest@localhost:5672/"
  }
}
```

## Message Flow Details

### 1. .NET Publishes Request
When a user submits a transcript request, .NET:
1. Creates domain event: `TranscriptRequestSubmittedDomainEvent`
2. Domain event handler publishes: `TranscriptRequestedIntegrationEvent`
3. MassTransit sends to RabbitMQ exchange: `m3-net-modules-transcripts-integration-events:transcript-requested-integration-event`

### 2. Python Consumes Request
transcript-consumer:
1. Receives message from RabbitMQ
2. Validates and parses `TranscriptRequestedIntegrationEvent`
3. Publishes progress event: "Starting processing"
4. Calls transcript-api: `POST /process-video-from-consumer`
5. Publishes completion/failure event based on result

### 3. .NET Handles Response
.NET receives progress/completion events and:
1. Logs the events
2. Updates database (implementation pending)
3. Notifies users (implementation pending)

## Error Handling

### Python Service Errors
- **Connection failures**: Automatic retry with exponential backoff
- **Processing errors**: Logged and sent as failure events to .NET
- **Invalid messages**: Rejected (not requeued) to prevent infinite loops

### .NET Service Errors
- **Consumer failures**: Dead letter queue handling
- **Database errors**: Logged for manual intervention
- **Invalid events**: Logged and ignored

## Monitoring and Observability

### Health Checks
- **transcript-consumer**: `GET http://localhost:8000/health`
- **transcript-api**: `GET http://localhost:8080/`
- **transcript-mcp**: MCP protocol health checks

### Logs
- **Python**: Structured JSON logs via Loguru
- **.NET**: Structured logs via Serilog
- **Integration**: Correlation IDs for tracing requests

### Metrics (if Opik configured)
- Request processing times
- Success/failure rates
- Token usage (Groq API)
- Memory usage patterns

## Troubleshooting

### Common Issues

#### RabbitMQ Connection Issues
```bash
# Check RabbitMQ status
rabbitmqctl status

# Check exchanges and queues
rabbitmqctl list_exchanges
rabbitmqctl list_queues
```

#### Python Service Issues
```bash
# Check logs
tail -f transcript-consumer.log
tail -f transcript-api.log

# Test individual services
curl http://localhost:8000/health
curl http://localhost:8080/docs
```

#### .NET Integration Issues
```bash
# Check .NET logs
dotnet run --verbosity detailed

# Test MassTransit configuration
# Check DI container registration
```

### Debug Mode
Enable debug logging in Python:
```python
import logging
logging.basicConfig(level=logging.DEBUG)
```

Enable debug logging in .NET:
```json
{
  "Logging": {
    "LogLevel": {
      "MassTransit": "Debug",
      "M3.Net.Modules.Transcripts": "Debug"
    }
  }
}
```

## Future Enhancements

### Planned Features
1. **Database Integration**: Save transcript content and status
2. **User Notifications**: SignalR for real-time updates
3. **Retry Logic**: Failed request retry mechanisms
4. **Saga Pattern**: Distributed transaction management
5. **Caching**: Redis caching for frequently accessed data

### Monitoring Improvements
1. **Prometheus Metrics**: Custom metrics for both .NET and Python
2. **Distributed Tracing**: OpenTelemetry integration
3. **Alerting**: Failure notifications and performance alerts

### Security Enhancements
1. **Authentication**: API key validation
2. **Authorization**: User-based access control
3. **Encryption**: Message encryption for sensitive data

## Testing Strategy

### Unit Tests
- Python: `pytest` for service logic
- .NET: `xUnit` for integration event handlers

### Integration Tests
- End-to-end message flow testing
- Database integration testing
- Error scenario testing

### Load Testing
- RabbitMQ throughput testing
- Python service performance testing
- .NET API scalability testing

---

**Integration Status**: ✅ Complete
**Build Status**: ✅ Passing
**Services Required**: RabbitMQ, PostgreSQL, Python 3.12+, .NET 9

For issues or questions, check the logs first, then refer to the troubleshooting section above.
