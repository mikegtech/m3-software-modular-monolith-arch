import json
import asyncio
import os
from datetime import datetime
from typing import Any
from uuid import UUID

import aio_pika
import aiofiles
import httpx
from pydantic import BaseModel, Field

from loguru import logger
logger = logger.bind(name="TranscriptsConsumer")

RABBITMQ_URL = os.environ.get("RABBITMQ_URL", "amqp://guest:guest@localhost:5672/")
TRANSCRIPT_API_URL = os.environ.get("TRANSCRIPT_API_URL", "http://localhost:8000")

# .NET MassTransit publishes to exchange names based on the event type in kebab-case
EXCHANGE_NAME = "m3-net-modules-transcripts-integration-events:transcript-requested-integration-event"
QUEUE_NAME = "transcript-consumer"

# Outbound exchange for publishing events back to .NET
OUTBOUND_EXCHANGE_PROGRESS = "m3-net-modules-transcripts-integration-events:transcript-processing-progress-integration-event"
OUTBOUND_EXCHANGE_COMPLETED = "m3-net-modules-transcripts-integration-events:transcript-processing-completed-integration-event"

STATE_PATH = os.getenv("STATE_PATH", "./state.json")  # State file path


class TranscriptRequestedIntegrationEvent(BaseModel):
    """Message contract matching .NET TranscriptRequestedIntegrationEvent"""
    id: UUID = Field(description="Event ID")
    occurred_on_utc: datetime = Field(description="When the event occurred", alias="occurredOnUtc")
    request_id: UUID = Field(description="Transcript request ID", alias="requestId")
    user_id: UUID = Field(description="User ID", alias="userId")
    you_tube_url: str = Field(description="YouTube URL", alias="youTubeUrl")
    title: str = Field(description="Video title")
    duration_seconds: int = Field(description="Video duration in seconds", alias="durationSeconds")

    class Config:
        populate_by_name = True


class TranscriptProcessingCompletedEvent(BaseModel):
    """Event to publish back to .NET when processing is complete"""
    id: UUID = Field(description="Event ID")
    occurred_on_utc: datetime = Field(description="When the event occurred")
    request_id: UUID = Field(description="Original transcript request ID")
    success: bool = Field(description="Whether processing was successful")
    transcript_content: str | None = Field(description="Processed transcript content")
    error_message: str | None = Field(description="Error message if failed")


class TranscriptProcessingProgressEvent(BaseModel):
    """Event to publish progress updates back to .NET"""
    id: UUID = Field(description="Event ID")
    occurred_on_utc: datetime = Field(description="When the event occurred")
    request_id: UUID = Field(description="Original transcript request ID")
    status: str = Field(description="Current status: processing, downloading, transcribing, etc.")
    progress_percentage: int = Field(description="Progress percentage (0-100)")
    message: str | None = Field(description="Optional status message")


class TranscriptsConsumer:
    connection: aio_pika.Connection
    channel: aio_pika.Channel
    outbound_exchange: aio_pika.Exchange
    processed_requests: dict[str, Any] = {}
    last_update: datetime = None
    subscribe_task: asyncio.Task | None = None
    http_client: httpx.AsyncClient | None = None

    async def start(self) -> None:
        logger.info("Starting Transcripts Consumer")
        await self.load_state()

        # Initialize HTTP client for transcript-api communication
        self.http_client = httpx.AsyncClient(
            base_url=TRANSCRIPT_API_URL,
            timeout=httpx.Timeout(300.0)  # 5 minutes for long-running operations
        )

        self.connection = await aio_pika.connect_robust(RABBITMQ_URL)
        self.channel = await self.connection.channel()
        
        # Declare exchange for incoming transcript requests
        exchange = await self.channel.declare_exchange(
            EXCHANGE_NAME, aio_pika.ExchangeType.FANOUT, durable=True
        )

        # Declare exchanges for outbound events (progress/completion)
        self.progress_exchange = await self.channel.declare_exchange(
            OUTBOUND_EXCHANGE_PROGRESS, aio_pika.ExchangeType.FANOUT, durable=True
        )
        
        self.completed_exchange = await self.channel.declare_exchange(
            OUTBOUND_EXCHANGE_COMPLETED, aio_pika.ExchangeType.FANOUT, durable=True
        )

        # Consumer queue for transcript requests
        arguments = {"x-queue-type": "quorum"}
        queue = await self.channel.declare_queue(
            QUEUE_NAME, durable=True, arguments=arguments
        )
        await queue.bind(exchange, routing_key="")
        logger.info(f" [*] Waiting for transcript requests on queue: {QUEUE_NAME}")
        self.subscribe_task = asyncio.create_task(self.subscribe(queue))

    async def stop(self) -> None:
        """Stops the processor."""
        logger.info("Stopping Transcripts Consumer")
        
        # Cancel the subscription task if it exists.
        if self.subscribe_task:
            self.subscribe_task.cancel()
            try:
                await self.subscribe_task
            except asyncio.CancelledError:
                logger.info("Subscription task cancelled.")
        
        # Close HTTP client
        if self.http_client:
            await self.http_client.aclose()
        
        await self.save_state()
        await self.channel.close()
        await self.connection.close()
        
    async def subscribe(self, queue: aio_pika.Queue):
        async with queue.iterator() as queue_iter:
            async for message in queue_iter:
                async with message.process():
                    try:
                        logger.debug(f"Received headers: {message.headers}")
                        logger.debug(f"Received message body: {message.body.decode()}")
                        await self.on_transcript_request(message.body)
                    except Exception as e:  # noqa: BLE001
                        logger.error(f"Error processing message: {e}")
                        # Don't requeue on parsing/processing errors - send to DLQ instead
                        await message.reject(requeue=False)
        logger.info(" [*] Waiting for transcript request messages...")

    async def on_transcript_request(self, message_body: bytes):
        """Process incoming transcript request from .NET"""
        try:
            # Parse the .NET integration event
            data = json.loads(message_body.decode())
            logger.info(f"Received transcript request: {data}")
            
            # Convert to our Pydantic model for validation
            request_event = TranscriptRequestedIntegrationEvent(**data)
            
            # Store the request
            self.processed_requests[str(request_event.request_id)] = {
                "event": request_event.dict(),
                "status": "received",
                "created_at": datetime.now(),
                "last_updated": datetime.now()
            }
            
            # Publish progress event: Starting processing
            await self.publish_progress_event(
                request_event.request_id,
                "processing",
                0,
                "Starting transcript processing"
            )
            
            # Process the transcript request asynchronously
            asyncio.create_task(self.process_transcript_request(request_event))
            
        except Exception as e:
            logger.error(f"Error parsing transcript request: {e}")
            raise

    async def process_transcript_request(self, request_event: TranscriptRequestedIntegrationEvent):
        """Process transcript request by calling transcript-api"""
        request_id = request_event.request_id
        
        try:
            # Update status
            self.processed_requests[str(request_id)]["status"] = "processing"
            self.processed_requests[str(request_id)]["last_updated"] = datetime.now()
            
            # Publish progress: Downloading video
            await self.publish_progress_event(
                request_id,
                "downloading",
                25,
                f"Downloading video from {request_event.you_tube_url}"
            )
            
            # Call transcript-api to process the video
            # Note: This assumes transcript-api has an endpoint that accepts YouTube URLs
            response = await self.http_client.post(
                "/process-video-from-consumer",
                json={
                    "video_path": request_event.you_tube_url,
                    "request_id": str(request_id),
                    "user_id": str(request_event.user_id)
                }
            )
            
            if response.status_code == 200:
                result = response.json()
                
                # Update progress: Processing complete
                await self.publish_progress_event(
                    request_id,
                    "completed",
                    100,
                    "Transcript processing completed successfully"
                )
                
                # Publish completion event
                await self.publish_completion_event(
                    request_id,
                    success=True,
                    transcript_content=result.get("transcript_content"),
                    error_message=None
                )
                
                # Update local state
                self.processed_requests[str(request_id)]["status"] = "completed"
                self.processed_requests[str(request_id)]["result"] = result
                
            else:
                error_msg = f"Transcript API error: {response.status_code} - {response.text}"
                logger.error(error_msg)
                
                await self.publish_completion_event(
                    request_id,
                    success=False,
                    transcript_content=None,
                    error_message=error_msg
                )
                
                self.processed_requests[str(request_id)]["status"] = "failed"
                self.processed_requests[str(request_id)]["error"] = error_msg
                
        except Exception as e:
            error_msg = f"Error processing transcript request {request_id}: {str(e)}"
            logger.error(error_msg)
            
            # Publish failure event
            await self.publish_completion_event(
                request_id,
                success=False,
                transcript_content=None,
                error_message=error_msg
            )
            
            # Update local state
            self.processed_requests[str(request_id)]["status"] = "failed"
            self.processed_requests[str(request_id)]["error"] = error_msg
        
        finally:
            self.processed_requests[str(request_id)]["last_updated"] = datetime.now()
            await self.save_state()

    async def publish_progress_event(self, request_id: UUID, status: str, progress: int, message: str = None):
        """Publish progress event back to .NET"""
        from uuid import uuid4
        
        progress_event = TranscriptProcessingProgressEvent(
            id=uuid4(),
            occurred_on_utc=datetime.utcnow(),
            request_id=request_id,
            status=status,
            progress_percentage=progress,
            message=message
        )
        
        await self.progress_exchange.publish(
            aio_pika.Message(
                progress_event.json().encode(),
                content_type="application/json"
            ),
            routing_key=""
        )
        
        logger.info(f"Published progress event for request {request_id}: {status} ({progress}%)")

    async def publish_completion_event(self, request_id: UUID, success: bool, transcript_content: str = None, error_message: str = None):
        """Publish completion event back to .NET"""
        from uuid import uuid4
        
        completion_event = TranscriptProcessingCompletedEvent(
            id=uuid4(),
            occurred_on_utc=datetime.utcnow(),
            request_id=request_id,
            success=success,
            transcript_content=transcript_content,
            error_message=error_message
        )
        
        await self.completed_exchange.publish(
            aio_pika.Message(
                completion_event.json().encode(),
                content_type="application/json"
            ),
            routing_key=""
        )
        
        logger.info(f"Published completion event for request {request_id}: success={success}")

    async def get_processed_requests(self) -> list[dict]:
        return list(self.processed_requests.values())

    async def info(self) -> dict[str, Any]:
        return {
            "requests_processed": len(self.processed_requests),
            "last_update": self.last_update,
            "status_counts": self._get_status_counts()
        }
    
    def _get_status_counts(self) -> dict[str, int]:
        """Get count of requests by status"""
        status_counts = {}
        for request_data in self.processed_requests.values():
            status = request_data.get("status", "unknown")
            status_counts[status] = status_counts.get(status, 0) + 1
        return status_counts

    async def save_state(self) -> None:
        """Persists the current state asynchronously, including all processed requests."""
        state = {
            "processed_requests": self.processed_requests,
            "last_update": self.last_update.isoformat() if self.last_update else None
        }
        try:
            async with aiofiles.open(STATE_PATH, "w") as f:
                await f.write(json.dumps(state, default=str))
        except Exception as e:
            logger.error(f"Failed to save state: {e}")

    async def load_state(self) -> None:
        """Loads the saved state asynchronously if available."""
        if os.path.exists(STATE_PATH):
            try:
                async with aiofiles.open(STATE_PATH, "r") as f:
                    content = await f.read()
                    state = json.loads(content)
                    self.processed_requests = state.get("processed_requests", {})
                    self.last_update = (
                        datetime.fromisoformat(state["last_update"])
                        if state["last_update"] else None
                    )
                    logger.info(f"State loaded asynchronously with {len(self.processed_requests)} processed requests.")
            except Exception as e:
                logger.error(f"Failed to load state: {e}")
        else:
            logger.info("No previous state found, starting fresh.")