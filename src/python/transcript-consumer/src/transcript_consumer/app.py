from contextlib import asynccontextmanager

from fastapi import FastAPI
from loguru import logger
from .transcripts_consumer import TranscriptsConsumer

logger = logger.bind(name="TranscriptsConsumerManager")

consumer = TranscriptsConsumer()

@asynccontextmanager
async def lifespan(app: FastAPI):  # noqa: ANN201, ARG001
    """https://fastapi.tiangolo.com/advanced/events/#lifespan."""
    logger.info("Transcripts Consumer: Starting up")
    await consumer.start()
    # FastAPI start excepting requests
    yield
    # FastAPI is shutting down
    logger.info("Transcripts Consumer: Shutting down")

    await consumer.stop()
    logger.info("Transcripts Consumer: Shut down complete")


app = FastAPI(
    title="Transcript Consumer Service",
    description="Consumer service for processing transcript requests from .NET M3 system",
    version="1.0.0",
    lifespan=lifespan
)

@app.get("/health")
async def health_check():
    """Health check endpoint for monitoring"""
    return {"status": "healthy", "service": "transcript-consumer"}

@app.get("/processed-requests")
async def list_processed_requests():
    """List all processed transcript requests"""
    result = await consumer.get_processed_requests()
    return {"processed_requests": result}

@app.get("/info")
async def info():
    """Get consumer statistics and information"""
    result = await consumer.info()
    return {"info": result}