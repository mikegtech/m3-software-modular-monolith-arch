# ADR-003: Event Storming Results - Transcripts Module Bounded Contexts

## Status
Proposed

## Date
2025-07-04

## Context

Following ADR-002, we need to conduct an event storming exercise to properly define the bounded contexts, domain events, aggregates, and business processes for the Transcripts module. This exercise will ensure we have a clear understanding of the domain before implementation begins.

The Transcripts module is responsible for managing YouTube video transcription workflows, from initial user requests through processing coordination to final transcript delivery.

## Event Storming Results

### Business Process Flow

```
User Request → Video Validation → Processing Queue → Transcription → Storage → Delivery
```

### Domain Events Identified

#### 1. Request & Validation Phase
- **TranscriptRequestSubmitted** - User submits a YouTube URL for transcription
- **VideoUrlValidated** - YouTube URL has been validated as accessible
- **VideoUrlRejected** - YouTube URL is invalid or inaccessible
- **VideoDurationChecked** - Video duration has been assessed for processing feasibility
- **VideoTooLongRejected** - Video exceeds maximum duration limits
- **DuplicateTranscriptDetected** - Same video has already been processed for this user

#### 2. Processing Coordination Phase
- **TranscriptProcessingQueued** - Request has been queued for processing
- **ProcessingJobCreated** - Background processing job has been created
- **ProcessingJobStarted** - Python microservice has begun processing
- **ProcessingProgressUpdated** - Progress updates during transcription
- **ProcessingJobFailed** - Processing failed due to technical issues
- **ProcessingJobTimedOut** - Processing exceeded maximum time limits

#### 3. Completion & Delivery Phase
- **TranscriptContentReceived** - Raw transcript content received from processor
- **TranscriptContentValidated** - Transcript content has been validated and formatted
- **TranscriptStorageCompleted** - Transcript has been persisted to database
- **TranscriptDeliveryReady** - Transcript is ready for user access
- **UserNotificationSent** - User has been notified of completion

#### 4. Management & Operations Phase
- **TranscriptDeleted** - User or system deleted a transcript
- **TranscriptShared** - Transcript shared with other users (future feature)
- **ProcessingResourcesReleased** - Cleanup after processing completion

### Bounded Contexts Identified

#### 1. **Transcript Request Management**
**Responsibility**: Managing user requests and validation
**Aggregates**: 
- `TranscriptRequest` (Root)
  - Properties: Id, UserId, YouTubeUrl, Title, RequestedAt, Status, ValidationResult
  - Behaviors: Submit(), Validate(), Reject(), Approve()

**Domain Events**:
- TranscriptRequestSubmitted
- VideoUrlValidated
- VideoUrlRejected
- VideoDurationChecked
- VideoTooLongRejected
- DuplicateTranscriptDetected

**Business Rules**:
- YouTube URL must be valid and accessible
- Video duration must not exceed 4 hours
- Users cannot request duplicate transcripts within 24 hours
- Free users limited to 5 requests per month

#### 2. **Processing Coordination**
**Responsibility**: Orchestrating the transcription processing workflow
**Aggregates**:
- `ProcessingJob` (Root)
  - Properties: Id, TranscriptRequestId, Status, StartedAt, CompletedAt, ProgressPercentage, ErrorDetails
  - Behaviors: Start(), UpdateProgress(), Complete(), Fail(), Timeout()

**Domain Events**:
- TranscriptProcessingQueued
- ProcessingJobCreated
- ProcessingJobStarted
- ProcessingProgressUpdated
- ProcessingJobFailed
- ProcessingJobTimedOut

**Business Rules**:
- Only one processing job per transcript request
- Processing timeout after 2 hours
- Failed jobs can be retried up to 3 times
- Progress updates required every 30 seconds

#### 3. **Transcript Content Management**
**Responsibility**: Managing the actual transcript content and metadata
**Aggregates**:
- `Transcript` (Root)
  - Properties: Id, RequestId, Content, Language, Confidence, WordCount, Duration, CreatedAt, UpdatedAt
  - Behaviors: Store(), Update(), Validate(), Delete(), Export()

**Domain Events**:
- TranscriptContentReceived
- TranscriptContentValidated
- TranscriptStorageCompleted
- TranscriptDeleted

**Business Rules**:
- Transcript content must be validated before storage
- Content retention period based on user subscription
- Automatic language detection required
- Minimum confidence threshold for acceptance

#### 4. **User Notification & Delivery**
**Responsibility**: Notifying users and managing transcript access
**Aggregates**:
- `TranscriptDelivery` (Root)
  - Properties: Id, TranscriptId, UserId, DeliveryMethod, DeliveredAt, AccessCount
  - Behaviors: Prepare(), Deliver(), Track(), Expire()

**Domain Events**:
- TranscriptDeliveryReady
- UserNotificationSent
- TranscriptAccessed
- DeliveryExpired

**Business Rules**:
- Users notified within 5 minutes of completion
- Transcript access tracked for analytics
- Download links expire after 7 days
- Email notifications for premium users only

### Cross-Cutting Concerns

#### Authentication & Authorization
- User must be authenticated for all operations
- Role-based permissions for admin operations
- Rate limiting per user tier

#### Audit & Compliance
- All operations logged for audit trail
- Data retention policies enforced
- GDPR compliance for user data

#### Integration Points
- YouTube API for video metadata
- Python microservice for processing
- Email service for notifications
- File storage for transcript exports

### Aggregate Relationships

```
TranscriptRequest (1) → (1) ProcessingJob
TranscriptRequest (1) → (0..1) Transcript
Transcript (1) → (1) TranscriptDelivery
ProcessingJob (1) → (0..1) Transcript
```

### Integration Events for Module Communication

#### Outbound Events (Published by Transcripts Module)
- **TranscriptRequestedIntegrationEvent** - New transcript request submitted
- **TranscriptCompletedIntegrationEvent** - Transcript processing completed
- **TranscriptFailedIntegrationEvent** - Transcript processing failed
- **UserTranscriptQuotaUpdatedIntegrationEvent** - User's transcript quota changed

#### Inbound Events (Consumed by Transcripts Module)
- **UserRegisteredIntegrationEvent** - New user registered (initialize quota)
- **UserSubscriptionChangedIntegrationEvent** - User tier changed (update limits)
- **ProcessingResultReceivedIntegrationEvent** - Python service completed processing

### Database Schema Implications

Based on the bounded contexts, the `transcripts` schema will contain:

```sql
-- Transcript Request Management
transcripts.transcript_requests
transcripts.validation_results

-- Processing Coordination  
transcripts.processing_jobs
transcripts.processing_progress

-- Transcript Content Management
transcripts.transcripts
transcripts.transcript_metadata

-- User Notification & Delivery
transcripts.transcript_deliveries
transcripts.notification_logs

-- Cross-cutting
transcripts.audit_logs
transcripts.user_quotas
```

## Decision

We will implement the Transcripts module with **four distinct bounded contexts**:

1. **Transcript Request Management** - Handles user requests and validation
2. **Processing Coordination** - Orchestrates the transcription workflow
3. **Transcript Content Management** - Manages the actual transcript data
4. **User Notification & Delivery** - Handles user communication and access

Each bounded context will be implemented as a separate aggregate within the single Transcripts module, maintaining clear separation of concerns while avoiding the complexity of separate modules.

### Implementation Strategy

#### Phase 1: Core Domain Implementation
1. Implement `TranscriptRequest` aggregate with validation logic
2. Create domain events for the request lifecycle
3. Setup basic API endpoints for request submission

#### Phase 2: Processing Coordination
1. Implement `ProcessingJob` aggregate
2. Create integration with Python microservice
3. Setup progress tracking and timeout handling

#### Phase 3: Content Management
1. Implement `Transcript` aggregate
2. Create storage and retrieval logic
3. Add content validation and formatting

#### Phase 4: Delivery & Notifications
1. Implement `TranscriptDelivery` aggregate
2. Setup notification system
3. Add access tracking and analytics

## Benefits

### Domain Clarity
- Clear separation of concerns within the domain
- Well-defined aggregate boundaries
- Explicit business rules and invariants

### Technical Benefits
- Single module deployment maintains simplicity
- Clear event flows for integration
- Proper encapsulation of business logic
- Testable aggregate behaviors

### Business Benefits
- Complete transcript workflow coverage
- Proper user experience management
- Scalable processing coordination
- Comprehensive audit trail

## Risks and Mitigations

### Risks
1. **Aggregate Complexity** - Multiple aggregates in one module
2. **Event Ordering** - Ensuring proper event sequence
3. **State Consistency** - Managing state across aggregates

### Mitigations
1. **Clear Boundaries** - Well-defined aggregate responsibilities
2. **Event Sourcing** - Consider for complex state management
3. **Saga Pattern** - For cross-aggregate workflows

## Success Criteria

1. **Domain Model Validation**
   - All business scenarios covered by domain events
   - Aggregate invariants properly enforced
   - Clear command/query separation

2. **Integration Validation**
   - Event flows work end-to-end
   - Python microservice integration functional
   - User notification system operational

3. **Performance Validation**
   - Request validation within 100ms
   - Processing coordination overhead minimal
   - Transcript storage and retrieval performant

## Next Steps

1. Review event storming results with domain experts
2. Validate business rules and constraints
3. Begin implementation of Phase 1 (Request Management)
4. Setup integration testing framework
5. Create detailed API specifications

## Related ADRs
- ADR-002: Transcripts Module Architecture and YouTube Video Processing
- ADR-004: Python Microservice Integration (Future)

## Notes
- Event storming session should be conducted with business stakeholders
- Domain events may evolve during implementation
- Consider eventual consistency patterns for cross-aggregate operations
- Monitor aggregate size and consider splitting if they become too complex
