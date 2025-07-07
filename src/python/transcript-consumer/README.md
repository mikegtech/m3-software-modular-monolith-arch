# Transcript Consumer Service

A Python-based message consumer service for processing transcript requests in the M3.Net modular monolith ecosystem. This service implements the distributed, asynchronous integration patterns defined in [ADR-006](../../docs/ADRs/ADR-006-Transactional-Outbox-Integration.md).

## Overview

The Transcript Consumer Service is a FastAPI-based microservice that:
- Consumes transcript processing requests from RabbitMQ message queues
- Processes YouTube video transcription using OpenAI Whisper API
- Publishes progress updates and results back to the .NET modular monolith
- Maintains persistent state for reliability and recovery

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                .NET Modular Monolith                        │
├─────────────────────────────────────────────────────────────┤
│  Domain Events → Outbox → Integration Events → RabbitMQ     │
└─────────────────────────┬───────────────────────────────────┘
                          │ transcript.requested
                          ▼
┌─────────────────────────────────────────────────────────────┐
│                  RabbitMQ Message Broker                    │
├─────────────────────────────────────────────────────────────┤
│  Exchanges: transcript.requests, transcript.progress,       │
│            transcript.results                               │
└─────────────────────────┬───────────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────────────┐
│              Python Transcript Consumer                     │
├─────────────────────────────────────────────────────────────┤
│  Message Consumer → Process Video → Publish Progress        │
└─────────────────────────┬───────────────────────────────────┘
                          │ progress/results
                          ▼
┌─────────────────────────────────────────────────────────────┐
│                 .NET Inbox Handlers                         │
├─────────────────────────────────────────────────────────────┤
│  Update Database → Trigger Domain Events                    │
└─────────────────────────────────────────────────────────────┘
```

## Features

### Current Implementation
- ✅ **FastAPI Web Framework**: RESTful API with automatic OpenAPI documentation
- ✅ **RabbitMQ Integration**: Robust message consumption with aio_pika
- ✅ **Persistent State Management**: Async file-based state persistence
- ✅ **Graceful Lifecycle Management**: Proper startup/shutdown handling
- ✅ **Error Handling**: Message processing with retry and reject logic
- ✅ **Health Monitoring**: Basic health and info endpoints
- ✅ **Docker Support**: Containerized deployment with uv package manager

### Planned Enhancements (Per ADR-006)
- 🔄 **YouTube Video Processing**: yt-dlp integration for video downloads
- 🔄 **OpenAI Whisper Integration**: Audio transcription with timestamp processing
- 🔄 **Progress Reporting**: Real-time progress updates via message publishing
- 🔄 **Idempotency**: Message deduplication and replay protection
- 🔄 **Enhanced Error Handling**: Dead letter queues and circuit breakers
- 🔄 **Metrics and Observability**: Prometheus metrics and structured logging

## Technology Stack

### Core Dependencies
- **Python 3.12+**: Modern Python with type hints and async support
- **FastAPI**: High-performance web framework with automatic API docs
- **aio_pika**: Async RabbitMQ client for message processing
- **aiofiles**: Async file I/O for state persistence
- **loguru**: Structured logging with async support

### Planned Dependencies (ADR-006)
- **yt-dlp**: YouTube video downloading (preferred over youtube-dl)
- **openai**: OpenAI Whisper API integration
- **ffmpeg-python**: Audio/video processing
- **pydantic**: Data validation and settings management

## Quick Start

### Prerequisites
- Python 3.12 or higher
- RabbitMQ server running (default: localhost:5672)
- uv package manager (recommended) or pip

### Installation

1. **Clone and navigate to the project**:
   ```bash
   cd src/python/transcript-consumer
   ```

2. **Install dependencies with uv** (recommended):
   ```bash
   uv sync
   ```

   Or with pip:
   ```bash
   pip install -e .
   ```

3. **Set environment variables**:
   ```bash
   export RABBITMQ_URL="amqp://guest:guest@localhost:5672/"
   export STATE_PATH="./state.json"
   ```

### Running the Service

#### Development Mode
```bash
# With uv
uv run uvicorn transcript_consumer.app:app --reload --host 0.0.0.0 --port 8080

# With pip
uvicorn transcript_consumer.app:app --reload --host 0.0.0.0 --port 8080
```

#### Production Mode
```bash
# With uv
uv run uvicorn transcript_consumer.app:app --host 0.0.0.0 --port 8080 --workers 4

# With pip
uvicorn transcript_consumer.app:app --host 0.0.0.0 --port 8080 --workers 4
```

#### Docker
```bash
# Build the image
docker build -t transcript-consumer .

# Run the container
docker run -p 8080:8080 \
  -e RABBITMQ_URL="amqp://guest:guest@host.docker.internal:5672/" \
  -e STATE_PATH="/app/data/state.json" \
  -v $(pwd)/data:/app/data \
  transcript-consumer
```

## API Endpoints

### Health and Monitoring

#### `GET /info`
Returns service information and processing statistics.

**Response:**
```json
{
  "info": {
    "items_consumed": 42,
    "last_update": "2025-01-07T10:30:00"
  }
}
```

#### `GET /list-items`
Returns all processed items (for debugging).

**Response:**
```json
{
  "items": [
    {
      "title": "Sample Item",
      "created_at": "2025-01-07T10:30:00"
    }
  ]
}
```

#### `GET /docs`
Automatic OpenAPI documentation (FastAPI built-in).

#### `GET /redoc`
Alternative API documentation interface.

## Configuration

### Environment Variables

| Variable | Description | Default | Required |
|----------|-------------|---------|----------|
| `RABBITMQ_URL` | RabbitMQ connection string | `amqp://guest:guest@localhost:5672/` | No |
| `STATE_PATH` | Path to state persistence file | `./state.json` | No |
| `LOG_LEVEL` | Logging level (DEBUG, INFO, WARNING, ERROR) | `INFO` | No |

### Message Queue Configuration

#### Current Exchange
- **Name**: `items-updates`
- **Type**: `fanout`
- **Durability**: `true`

#### Current Queue
- **Name**: `consumer`
- **Type**: `quorum` (for high availability)
- **Durability**: `true`

### Planned Configuration (ADR-006)

#### Exchanges
- `transcript.requests` (direct) - Incoming transcript requests
- `transcript.progress` (direct) - Progress updates to .NET
- `transcript.results` (direct) - Completion/failure results

#### Queues
- `transcript.processing.queue` - Main processing queue
- `transcript.progress.inbox` - .NET progress updates
- `transcript.results.inbox` - .NET completion results

## State Management

The service maintains persistent state to ensure reliability:

### State File Format
```json
{
  "items": {
    "item-id-1": {
      "title": "Sample Item",
      "created_at": "2025-01-07T10:30:00"
    }
  },
  "last_update": "2025-01-07T10:30:00"
}
```

### State Operations
- **Save State**: Automatic after each message processing
- **Load State**: Automatic on service startup
- **Recovery**: Service resumes from last known state after restart

## Message Processing

### Current Flow
1. **Connect** to RabbitMQ with robust connection handling
2. **Declare** exchange and queue with proper durability settings
3. **Subscribe** to queue with async message iteration
4. **Process** each message with error handling
5. **Update** internal state and persist to file
6. **Acknowledge** successful processing or reject with requeue

### Message Format (Current)
```json
{
  "id": "unique-item-id",
  "title": "Item Title",
  "created_at": "2025-01-07T10:30:00"
}
```

### Planned Message Format (ADR-006)
```json
{
  "RequestId": "guid",
  "UserId": "user-guid",
  "YouTubeUrl": "https://youtube.com/watch?v=...",
  "RequestedAt": "2025-01-07T10:30:00"
}
```

## Error Handling

### Current Strategy
- **Message Processing Errors**: Log error and reject with requeue
- **State Persistence Errors**: Log error but continue processing
- **Connection Errors**: Automatic reconnection via aio_pika robust connection

### Planned Enhancements (ADR-006)
- **Dead Letter Queues**: Failed messages routed to DLQ for analysis
- **Circuit Breaker**: Prevent cascade failures during external API issues
- **Exponential Backoff**: Smart retry delays for transient failures
- **Poison Message Detection**: Identify and isolate problematic messages

## Monitoring and Observability

### Current Capabilities
- **Structured Logging**: loguru with contextual information
- **Basic Metrics**: Item count and last update timestamp
- **Health Endpoints**: Service status and processing info

### Planned Enhancements (ADR-006)
- **Prometheus Metrics**: Processing rates, error rates, queue depths
- **Distributed Tracing**: Request correlation across service boundaries
- **Queue Monitoring**: RabbitMQ queue depth and consumer lag
- **Performance Metrics**: Processing time, memory usage, throughput

## Development

### Project Structure
```
transcript-consumer/
├── src/
│   └── transcript_consumer/
│       ├── __init__.py          # Logger configuration
│       ├── app.py               # FastAPI application
│       └── transcripts_consumer.py  # Core consumer logic
├── pyproject.toml               # Project configuration
├── Dockerfile                   # Container configuration
└── README.md                    # This file
```

### Code Style
- **Type Hints**: Full type annotation with Python 3.12+ syntax
- **Async/Await**: Consistent async programming patterns
- **Error Handling**: Explicit exception handling with logging
- **Code Quality**: Linting with ruff (configured in pyproject.toml)

### Adding New Features

1. **Update Dependencies**: Add to `pyproject.toml`
2. **Implement Logic**: Add to `transcripts_consumer.py`
3. **Add Endpoints**: Extend `app.py` if needed
4. **Update Configuration**: Environment variables and settings
5. **Add Tests**: Unit and integration tests
6. **Update Documentation**: README and API docs

## Testing

### Running Tests (Planned)
```bash
# Unit tests
uv run pytest tests/unit/

# Integration tests (requires RabbitMQ)
uv run pytest tests/integration/

# All tests
uv run pytest
```

### Test Categories (Planned)
- **Unit Tests**: Message processing logic, state management
- **Integration Tests**: RabbitMQ communication, end-to-end flows
- **Performance Tests**: Load testing, memory usage, throughput
- **Reliability Tests**: Service restart, network failures, message replay

## Deployment

### Docker Deployment
```bash
# Production build
docker build -t transcript-consumer:latest .

# Docker Compose (with RabbitMQ)
docker-compose up -d
```

### Kubernetes Deployment (Planned)
- **Deployment**: Scalable pod configuration
- **Service**: Internal service discovery
- **ConfigMap**: Environment-specific configuration
- **Secret**: Sensitive configuration (API keys)
- **HPA**: Horizontal pod autoscaling based on queue depth

## Security Considerations

### Current Implementation
- **No Authentication**: Service runs without authentication (internal use)
- **Environment Variables**: Sensitive config via environment

### Planned Security (ADR-006)
- **API Keys**: Secure OpenAI API key management
- **TLS/SSL**: Encrypted RabbitMQ connections
- **Service Mesh**: mTLS for service-to-service communication
- **Secret Management**: Kubernetes secrets or external secret stores

## Performance Tuning

### Current Configuration
- **Connection Pooling**: Single robust connection per service instance
- **Queue Settings**: Quorum queues for high availability
- **Message Prefetch**: Default prefetch for flow control

### Planned Optimizations (ADR-006)
- **Concurrent Processing**: Multiple worker threads/processes
- **Resource Limits**: Memory and CPU constraints
- **Queue Optimization**: Prefetch tuning, lazy queues
- **Caching**: Response caching for repeated requests

## Troubleshooting

### Common Issues

#### Service Won't Start
```bash
# Check RabbitMQ connectivity
curl -u guest:guest http://localhost:15672/api/overview

# Check logs
docker logs transcript-consumer

# Verify environment variables
echo $RABBITMQ_URL
```

#### Messages Not Processing
```bash
# Check queue status
rabbitmqctl list_queues name messages consumers

# Check service logs
tail -f logs/transcript-consumer.log

# Verify exchange bindings
rabbitmqctl list_bindings
```

#### State Persistence Issues
```bash
# Check file permissions
ls -la state.json

# Verify disk space
df -h

# Check state file format
cat state.json | jq .
```

## Contributing

### Development Setup
1. Fork the repository
2. Create a feature branch
3. Install dependencies: `uv sync`
4. Make changes with tests
5. Run linting: `uv run ruff check`
6. Submit pull request

### Coding Standards
- Follow PEP 8 style guidelines
- Use type hints consistently
- Write comprehensive docstrings
- Add unit tests for new features
- Update README for API changes

## Roadmap

### Phase 1: Foundation (Current)
- ✅ Basic RabbitMQ consumer
- ✅ FastAPI framework
- ✅ State persistence
- ✅ Docker support

### Phase 2: Transcript Processing (Planned - Week 1-2)
- 🔄 YouTube video download (yt-dlp)
- 🔄 OpenAI Whisper API integration
- 🔄 Audio processing pipeline
- 🔄 Progress reporting

### Phase 3: Production Ready (Planned - Week 2-3)
- 🔄 Enhanced error handling
- 🔄 Metrics and monitoring
- 🔄 Performance optimization
- 🔄 Comprehensive testing

### Phase 4: Scale and Optimize (Planned - Week 3-4)
- 🔄 Horizontal scaling
- 🔄 Advanced queue management
- 🔄 Cost optimization
- 🔄 Operational runbooks

## Related Documentation

- [ADR-002: Transcripts Module Architecture](../../docs/ADRs/ADR-002-Transcripts-Module-Architecture.md)
- [ADR-004: Python Transcript Processing Service](../../docs/ADRs/ADR-004-Python-Transcript-Processing-Service.md)
- [ADR-005: YouTube Whisper Tutorial Analysis](../../docs/ADRs/ADR-005-YouTube-Whisper-Tutorial-Analysis.md)
- [ADR-006: Transactional Outbox Integration](../../docs/ADRs/ADR-006-Transactional-Outbox-Integration.md)

## License

This project is part of the M3.Net modular monolith and follows the same licensing terms.

## Support

For questions, issues, or contributions:
- Create GitHub issues for bugs and feature requests
- Review ADR documents for architectural decisions
- Check logs and monitoring for operational issues
- Consult the troubleshooting section for common problems
