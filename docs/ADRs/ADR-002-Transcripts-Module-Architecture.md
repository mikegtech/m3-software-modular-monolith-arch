# ADR-002: Transcripts Module Architecture and YouTube Video Processing

## Status
Proposed

## Date
2025-07-04

## Context

The M3.Net platform requires a new module to handle YouTube video transcription capabilities. This module will enable users to submit YouTube videos for transcription processing while maintaining the established modular monolith architecture patterns.

### Business Requirements
- Users need the ability to transcribe YouTube videos
- The system should handle video processing asynchronously
- Transcription results must be stored and retrievable
- The processing should be scalable and maintainable

### Technical Constraints
- Must follow the established Clean Architecture pattern
- Must maintain logical isolation between modules
- Processing workload is computationally intensive and better suited for Python ecosystem
- Must integrate with existing authentication and authorization system
- Database isolation must be maintained with separate schema

## Decision

We will implement a **Transcripts Module** following the established modular monolith pattern with **hybrid architecture** where .NET handles API orchestration and data management while Python microservice handles the actual video processing.

### Architecture Components

#### 1. Transcripts Module (.NET)
**Bounded Context**: YouTube Video Transcription Management
- **Domain Layer**: Transcript entities, domain events, business rules
- **Application Layer**: CQRS commands/queries, orchestration logic
- **Infrastructure Layer**: Database access, external service integration
- **Presentation Layer**: Minimal API endpoints integrated into M3.Net.Api
- **Database Schema**: `transcripts`

#### 2. Python Microservice
**Responsibility**: YouTube Video Processing
- Video downloading and processing
- Speech-to-text transcription
- Result formatting and delivery
- Scalable processing pipeline

### Integration Pattern

```
Client Request → M3.Net.Api → Transcripts Module → Domain Events → Python Microservice
                                      ↓
Database (transcripts schema) ← Domain Events ← Processing Results
```

## Implementation Strategy

### Phase 1: .NET Module Foundation
1. **Event Storming Session**
   - Define bounded context boundaries
   - Identify aggregates: `Transcript`, `VideoRequest`, `ProcessingJob`
   - Map domain events: `TranscriptRequested`, `ProcessingStarted`, `TranscriptCompleted`, `ProcessingFailed`

2. **Module Structure**
   ```
   src/Modules/Transcripts/
   ├── M3.Net.Modules.Transcripts.Domain/
   │   ├── Transcripts/
   │   │   ├── Transcript.cs
   │   │   ├── VideoRequest.cs
   │   │   ├── ProcessingJob.cs
   │   │   ├── TranscriptStatus.cs (enum)
   │   │   └── ITranscriptRepository.cs
   │   └── Events/
   │       ├── TranscriptRequestedDomainEvent.cs
   │       ├── ProcessingStartedDomainEvent.cs
   │       ├── TranscriptCompletedDomainEvent.cs
   │       └── ProcessingFailedDomainEvent.cs
   ├── M3.Net.Modules.Transcripts.Application/
   │   ├── Commands/
   │   │   ├── RequestTranscript/
   │   │   └── UpdateProcessingStatus/
   │   ├── Queries/
   │   │   ├── GetTranscript/
   │   │   └── GetUserTranscripts/
   │   └── Handlers/
   ├── M3.Net.Modules.Transcripts.Infrastructure/
   │   ├── Database/
   │   │   ├── TranscriptsDbContext.cs
   │   │   └── Migrations/
   │   ├── Repositories/
   │   └── Services/
   ├── M3.Net.Modules.Transcripts.Presentation/
   │   └── Endpoints/
   │       ├── RequestTranscript.cs
   │       ├── GetTranscript.cs
   │       └── GetUserTranscripts.cs
   └── M3.Net.Modules.Transcripts.IntegrationEvents/
       ├── TranscriptRequestedIntegrationEvent.cs
       ├── ProcessingCompletedIntegrationEvent.cs
       └── ProcessingFailedIntegrationEvent.cs
   ```

3. **API Endpoints Integration**
   - Register endpoints in M3.Net.Api startup
   - Follow existing Users module pattern
   - Maintain RESTful conventions

### Phase 2: Python Microservice Integration

1. **Message Broker Communication**
   - Use existing RabbitMQ infrastructure
   - Publish integration events from .NET
   - Consume events in Python microservice
   - Publish results back via integration events

2. **Event Flow**
   ```
   1. User submits YouTube URL → TranscriptRequested domain event
   2. Domain event handler publishes TranscriptRequestedIntegrationEvent
   3. Python service consumes integration event
   4. Python service processes video and publishes results
   5. .NET consumes result events and updates database
   ```

### Phase 3: Database Schema Design

```sql
-- transcripts schema
CREATE SCHEMA transcripts;

-- Core transcript entity
CREATE TABLE transcripts.transcripts (
    id UUID PRIMARY KEY,
    user_id UUID NOT NULL,
    youtube_url VARCHAR(500) NOT NULL,
    title VARCHAR(200),
    transcript_text TEXT,
    status VARCHAR(50) NOT NULL, -- Requested, Processing, Completed, Failed
    duration_seconds INTEGER,
    created_on_utc TIMESTAMP WITH TIME ZONE NOT NULL,
    completed_on_utc TIMESTAMP WITH TIME ZONE,
    error_message TEXT
);

-- Processing jobs for tracking
CREATE TABLE transcripts.processing_jobs (
    id UUID PRIMARY KEY,
    transcript_id UUID NOT NULL REFERENCES transcripts.transcripts(id),
    status VARCHAR(50) NOT NULL,
    started_on_utc TIMESTAMP WITH TIME ZONE,
    completed_on_utc TIMESTAMP WITH TIME ZONE,
    error_details TEXT
);

-- Outbox and Inbox tables (following established pattern)
-- ... (standard outbox/inbox pattern tables)
```

## Benefits

### Architectural Benefits
- **Separation of Concerns**: .NET handles orchestration, Python handles processing
- **Technology Optimization**: Use Python's superior ML/AI ecosystem for transcription
- **Scalability**: Python microservice can be scaled independently
- **Maintainability**: Clear boundaries between API management and processing logic

### Business Benefits
- **Performance**: Specialized tools for video processing
- **Reliability**: Asynchronous processing with proper error handling
- **Monitoring**: Clear observability across both components
- **Cost Efficiency**: Can scale processing resources independently

## Risks and Mitigations

### Risks
1. **Network Latency**: Communication between .NET and Python services
2. **Processing Failures**: Video processing can fail for various reasons
3. **Data Consistency**: Ensuring consistency across service boundaries
4. **Operational Complexity**: Managing multiple technology stacks

### Mitigations
1. **Async Processing**: Use message queues for resilient communication
2. **Retry Logic**: Implement exponential backoff and dead letter queues
3. **Saga Pattern**: Use domain events for eventual consistency
4. **Monitoring**: Comprehensive logging and health checks across services

## Implementation Timeline

### Milestone 1 (Week 1-2): Event Storming and Design
- Conduct event storming session
- Finalize domain model and events
- Design database schema
- Define integration contracts

### Milestone 2 (Week 3-4): .NET Module Implementation
- Implement domain layer
- Create application layer with CQRS
- Setup database with migrations
- Implement presentation layer endpoints

### Milestone 3 (Week 5-6): Integration and Testing
- Integrate with M3.Net.Api
- Setup message broker communication
- Implement comprehensive testing
- Performance testing and optimization

### Milestone 4 (Week 7-8): Python Microservice (Separate ADR)
- Python service implementation (detailed in separate ADR)
- End-to-end integration testing
- Production deployment preparation

## Success Criteria

1. **Functional Requirements**
   - Users can submit YouTube URLs for transcription
   - System processes videos asynchronously
   - Transcription results are stored and retrievable
   - Proper error handling and user feedback

2. **Non-Functional Requirements**
   - API response time < 200ms for submission
   - Processing completion within reasonable time bounds
   - 99.9% availability for API endpoints
   - Comprehensive logging and monitoring

3. **Architectural Requirements**
   - Module follows Clean Architecture principles
   - Database isolation maintained
   - Integration events working correctly
   - Architecture tests passing

## Related ADRs
- ADR-001: [Previous ADR if exists]
- ADR-003: Python Microservice Architecture (Future)

## Notes
- This ADR focuses on the .NET module architecture
- Python microservice architecture will be covered in a separate ADR
- Event storming session required before implementation begins
- Consider YouTube API rate limits and terms of service compliance
