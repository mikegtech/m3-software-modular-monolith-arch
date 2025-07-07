# Transcript API

An AI-powered sports assistant API that provides intelligent video analysis, transcript processing, and conversational interactions using advanced language models and video processing capabilities.

## Overview

The Transcript API is a FastAPI-based microservice that combines:
- **AI Agent System**: Groq-powered conversational AI with memory and tool capabilities
- **Video Processing**: Background video analysis and clip generation
- **MCP Integration**: Model Context Protocol (MCP) server communication for advanced video tools
- **Memory Management**: Persistent conversation history using PixelTable
- **Observability**: Comprehensive tracking with Opik/Comet ML integration

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                     Client Applications                     │
├─────────────────────────────────────────────────────────────┤
│  Web UI, Mobile Apps, API Consumers                        │
└─────────────────────────┬───────────────────────────────────┘
                          │ HTTP/REST
                          ▼
┌─────────────────────────────────────────────────────────────┐
│                    Transcript API                           │
├─────────────────────────────────────────────────────────────┤
│  FastAPI + CORS + Static Files + Background Tasks          │
└─────────────────────────┬───────────────────────────────────┘
                          │
          ┌───────────────┴───────────────┐
          │                               │
          ▼                               ▼
┌──────────────────────┐         ┌─────────────────────┐
│     Groq Agent       │         │   MCP Server        │
├──────────────────────┤         ├─────────────────────┤
│ • LLM Models         │◄────────┤ • Video Processing  │
│ • Memory System      │         │ • Tool Definitions  │
│ • Tool Routing       │         │ • Clip Generation   │
│ • Response Generation│         │ • Transcript Tools  │
└──────────────────────┘         └─────────────────────┘
          │
          ▼
┌─────────────────────────────────────────────────────────────┐
│                   External Services                         │
├─────────────────────────────────────────────────────────────┤
│  Groq API • PixelTable • Opik/Comet ML • File Storage      │
└─────────────────────────────────────────────────────────────┘
```

## Features

### 🤖 **AI Agent Capabilities**
- **Multi-Model Support**: Various Groq models for different tasks (routing, tool use, image analysis)
- **Intelligent Routing**: Determines when to use tools vs. direct responses
- **Memory Management**: Persistent conversation history with configurable size
- **Tool Integration**: Dynamic tool discovery and execution via MCP
- **Image Analysis**: Base64 image processing and analysis

### 🎥 **Video Processing**
- **Video Upload**: Secure file upload with validation
- **Background Processing**: Asynchronous video analysis tasks
- **Clip Generation**: Extract relevant video segments
- **Media Serving**: Efficient static file serving
- **Progress Tracking**: Real-time task status monitoring

### 🔧 **Technical Features**
- **FastAPI Framework**: High-performance async API with automatic documentation
- **CORS Support**: Cross-origin resource sharing for web clients
- **Background Tasks**: Non-blocking video processing
- **Error Handling**: Comprehensive exception handling and logging
- **Observability**: Detailed tracking and monitoring with Opik

### 📊 **Observability & Monitoring**
- **Request Tracking**: All interactions tracked with Opik decorators
- **Performance Metrics**: Response times, token usage, error rates
- **Memory Analytics**: Conversation flow and agent decision tracking
- **Tool Usage**: Detailed tool execution monitoring

## Technology Stack

### Core Dependencies
- **Python 3.12+**: Modern Python with full type hint support
- **FastAPI**: High-performance web framework with automatic API documentation
- **Groq**: Advanced language model API client
- **Instructor**: Structured LLM outputs with Pydantic models
- **FastMCP**: Model Context Protocol client for tool integration
- **PixelTable**: Vector database for conversation memory storage
- **Opik**: Observability and tracking for AI applications
- **Loguru**: Advanced structured logging

### Development Tools
- **Click**: Command-line interface creation
- **Ruff**: Fast Python linting and formatting
- **Pydantic**: Data validation and settings management
- **Uvicorn**: ASGI server for FastAPI applications

## Quick Start

### Prerequisites
- Python 3.12 or higher
- Groq API key
- MCP server (transcript-mcp service)
- Optional: Opik API key for observability

### Installation

1. **Clone and navigate to the project**:
   ```bash
   cd src/python/transcript-api
   ```

2. **Install dependencies with uv** (recommended):
   ```bash
   uv sync
   ```

   Or with pip:
   ```bash
   pip install -e .
   ```

3. **Configure environment variables**:
   ```bash
   cp .env.example .env
   # Edit .env with your API keys
   ```

### Configuration

Create a `.env` file with the following variables:

```env
# Required
GROQ_API_KEY=your_groq_api_key_here

# Optional - for observability
OPIK_API_KEY=your_opik_api_key_here
OPIK_PROJECT=transcript-api

# MCP Server Configuration
MCP_SERVER=http://transcript-mcp:9090/mcp

# Model Configuration (optional - defaults provided)
GROQ_ROUTING_MODEL=meta-llama/llama-4-scout-17b-16e-instruct
GROQ_TOOL_USE_MODEL=meta-llama/llama-4-maverick-17b-128e-instruct
GROQ_IMAGE_MODEL=meta-llama/llama-4-maverick-17b-128e-instruct
GROQ_GENERAL_MODEL=meta-llama/llama-4-maverick-17b-128e-instruct

# Memory Configuration
AGENT_MEMORY_SIZE=20
```

### Running the Service

#### Development Mode
```bash
# With uv
uv run python src/transcript_api/api.py --port 8080 --host 0.0.0.0

# With uvicorn directly
uvicorn transcript_api.api:app --reload --host 0.0.0.0 --port 8080
```

#### Production Mode
```bash
# With uv
uv run python src/transcript_api/api.py --port 8080 --host 0.0.0.0

# With gunicorn (for production deployment)
gunicorn transcript_api.api:app -w 4 -k uvicorn.workers.UvicornWorker --bind 0.0.0.0:8080
```

#### Docker
```bash
# Build the image
docker build -t transcript-api .

# Run the container
docker run -p 8080:8080 \
  -e GROQ_API_KEY=your_key_here \
  -e MCP_SERVER=http://transcript-mcp:9090/mcp \
  -v $(pwd)/shared_media:/app/shared_media \
  transcript-api
```

## API Endpoints

### 📝 **Chat & Conversation**

#### `POST /chat`
Interactive chat with the AI assistant.

**Request:**
```json
{
  "message": "Can you analyze this video?",
  "video_path": "shared_media/video.mp4",
  "image_base64": "optional_base64_encoded_image"
}
```

**Response:**
```json
{
  "message": "I've analyzed the video and found several interesting highlights...",
  "clip_path": "shared_media/generated_clip.mp4"
}
```

#### `POST /reset-memory`
Reset the conversation memory.

**Response:**
```json
{
  "message": "Memory reset successfully"
}
```

### 🎥 **Video Processing**

#### `POST /upload-video`
Upload a video file for processing.

**Request:** Multipart form data with video file

**Response:**
```json
{
  "message": "Video uploaded successfully",
  "video_path": "shared_media/uploaded_video.mp4"
}
```

#### `POST /process-video`
Start background video processing.

**Request:**
```json
{
  "video_path": "shared_media/video.mp4"
}
```

**Response:**
```json
{
  "message": "Task enqueued for processing",
  "task_id": "uuid-task-identifier"
}
```

#### `GET /task-status/{task_id}`
Check processing task status.

**Response:**
```json
{
  "task_id": "uuid-task-identifier",
  "status": "completed"
}
```

**Status Values:** `pending`, `in_progress`, `completed`, `failed`, `not_found`

### 📁 **Media & Static Files**

#### `GET /media/{file_path}`
Serve processed media files.

**Example:** `GET /media/generated_clip.mp4`

### 📚 **Documentation**

#### `GET /docs`
Interactive API documentation (Swagger UI)

#### `GET /redoc`
Alternative API documentation (ReDoc)

#### `GET /`
API welcome message and documentation links

## Agent System

### 🧠 **Multi-Model Architecture**

The Groq agent uses different models for different tasks:

- **Routing Model**: `llama-4-scout-17b-16e-instruct` - Determines if tools are needed
- **Tool Use Model**: `llama-4-maverick-17b-128e-instruct` - Executes tool calls
- **Image Model**: `llama-4-maverick-17b-128e-instruct` - Processes image inputs
- **General Model**: `llama-4-maverick-17b-128e-instruct` - General conversation

### 🔧 **Tool Integration**

Tools are dynamically discovered from the MCP server and include:
- Video processing and analysis
- Clip generation and extraction
- Transcript analysis
- Image-based video search
- Custom sports analysis tools

### 💾 **Memory System**

**PixelTable-based persistent memory:**
- Stores conversation history across sessions
- Configurable memory size (default: 20 messages)
- Efficient retrieval of recent context
- Thread-based conversation tracking

**Memory Operations:**
```python
# Memory automatically manages:
memory_record = MemoryRecord(
    message_id="uuid",
    role="user|assistant",
    content="message content",
    timestamp=datetime.now()
)
```

### 🎯 **Response Generation**

**Structured Outputs with Pydantic:**
- `RoutingResponseModel`: Tool usage decisions
- `GeneralResponseModel`: Direct conversational responses
- `VideoClipResponseModel`: Video-specific responses with clips

## Configuration Reference

### Environment Variables

| Variable | Description | Default | Required |
|----------|-------------|---------|----------|
| `GROQ_API_KEY` | Groq API authentication key | - | ✅ |
| `GROQ_ROUTING_MODEL` | Model for routing decisions | `llama-4-scout-17b-16e-instruct` | ❌ |
| `GROQ_TOOL_USE_MODEL` | Model for tool execution | `llama-4-maverick-17b-128e-instruct` | ❌ |
| `GROQ_IMAGE_MODEL` | Model for image analysis | `llama-4-maverick-17b-128e-instruct` | ❌ |
| `GROQ_GENERAL_MODEL` | Model for general chat | `llama-4-maverick-17b-128e-instruct` | ❌ |
| `OPIK_API_KEY` | Opik observability API key | `None` | ❌ |
| `OPIK_WORKSPACE` | Opik workspace name | `default` | ❌ |
| `OPIK_PROJECT` | Opik project name | `transcript-api` | ❌ |
| `AGENT_MEMORY_SIZE` | Number of messages to retain | `20` | ❌ |
| `MCP_SERVER` | MCP server endpoint | `http://transcript-mcp:9090/mcp` | ❌ |
| `DISABLE_NEST_ASYNCIO` | Disable nested asyncio | `True` | ❌ |

### Model Selection

**Routing Model (Scout):**
- Optimized for classification tasks
- Fast inference for tool/no-tool decisions
- Lightweight and cost-effective

**Tool Use Model (Maverick):**
- Excellent function calling capabilities
- Structured output generation
- Complex reasoning for tool parameters

**Image Model (Maverick):**
- Vision capabilities for image analysis
- Video frame understanding
- Multi-modal reasoning

## Observability

### 🔍 **Opik Integration**

Comprehensive tracking of:
- **Chat Interactions**: Complete conversation flows
- **Tool Executions**: Tool calls and responses
- **Model Performance**: Token usage, latency, success rates
- **Memory Operations**: Conversation history management

### 📊 **Tracking Decorators**

```python
@opik.track(name="chat-interaction", type="llm")
async def chat(self, message: str, video_path: str, image_base64: str) -> AssistantMessageResponse:
    # Automatically tracked with Opik
```

### 📈 **Metrics Available**

- **Response Times**: Per endpoint and model
- **Token Usage**: Input/output tokens per model
- **Error Rates**: Failed requests and exceptions
- **Tool Usage**: Frequency and success of tool calls
- **Memory Stats**: Conversation length and retention

### 🚨 **Logging**

**Structured logging with Loguru:**
```python
logger.bind(name="GroqAgent").info("Processing chat request", 
    user_id=user_id, 
    message_length=len(message),
    has_image=bool(image_base64)
)
```

## Development

### 📁 **Project Structure**
```
transcript-api/
├── src/
│   └── transcript_api/
│       ├── agent/                    # AI agent implementation
│       │   ├── base_agent.py        # Abstract base agent
│       │   ├── memory.py            # PixelTable memory system
│       │   ├── groq/                # Groq-specific implementation
│       │   │   ├── groq_agent.py    # Main Groq agent
│       │   │   └── groq_tool.py     # Tool transformation
│       │   └── __init__.py
│       ├── api.py                   # FastAPI application
│       ├── config.py                # Configuration management
│       ├── models.py                # Pydantic models
│       ├── opik_utils.py           # Observability utilities
│       └── __init__.py
├── shared_media/                    # Media file storage
├── .env                            # Environment configuration
├── .dockerignore                   # Docker ignore rules
├── Dockerfile                      # Container configuration
├── pyproject.toml                  # Project dependencies
└── README.md                       # This file
```

### 🔧 **Code Style**

- **Type Hints**: Full type annotation throughout
- **Async/Await**: Consistent async programming patterns
- **Pydantic Models**: Data validation and serialization
- **Error Handling**: Comprehensive exception management
- **Logging**: Structured logging with context

### 🧪 **Adding New Features**

1. **New API Endpoints**: Add to `api.py` with proper models
2. **Agent Capabilities**: Extend `BaseAgent` or `GroqAgent`
3. **Memory Features**: Modify `Memory` class for new storage patterns
4. **Tool Integration**: Update MCP client interactions
5. **Models**: Add Pydantic models in `models.py`
6. **Configuration**: Add settings to `config.py`

### 🔍 **Debugging**

**Enable debug logging:**
```python
import logging
logging.basicConfig(level=logging.DEBUG)
```

**Opik debugging:**
```python
# Check tracking data
import opik
opik.get_current_trace()
```

**Memory inspection:**
```python
# View conversation history
memory = agent.memory
recent_messages = memory.get_latest(10)
```

## Testing

### 🧪 **Testing Strategy** (Planned)

```bash
# Unit tests
uv run pytest tests/unit/

# Integration tests (requires MCP server)
uv run pytest tests/integration/

# API tests
uv run pytest tests/api/

# All tests
uv run pytest
```

### 📝 **Test Categories** (Planned)

- **Unit Tests**: Agent logic, memory operations, model transformations
- **Integration Tests**: MCP server communication, end-to-end flows
- **API Tests**: FastAPI endpoint validation, request/response formats
- **Performance Tests**: Response times, concurrent users, memory usage

### 🎯 **Test Examples** (Planned)

```python
async def test_chat_with_video():
    """Test chat interaction with video processing"""
    response = await client.post("/chat", json={
        "message": "Analyze this video",
        "video_path": "test_video.mp4"
    })
    assert response.status_code == 200
    assert "clip_path" in response.json()

async def test_memory_persistence():
    """Test conversation memory across requests"""
    # First message
    await agent.chat("Hello", None, None)
    
    # Second message should have context
    response = await agent.chat("What did I just say?", None, None)
    assert "hello" in response.message.lower()
```

## Deployment

### 🐳 **Docker Deployment**

**Production container:**
```bash
# Build optimized image
docker build -t transcript-api:latest .

# Run with production settings
docker run -d \
  --name transcript-api \
  -p 8080:8080 \
  -e GROQ_API_KEY=your_key \
  -e MCP_SERVER=http://transcript-mcp:9090/mcp \
  -v /path/to/media:/app/shared_media \
  --restart unless-stopped \
  transcript-api:latest
```

### ☸️ **Kubernetes Deployment** (Planned)

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: transcript-api
spec:
  replicas: 3
  selector:
    matchLabels:
      app: transcript-api
  template:
    metadata:
      labels:
        app: transcript-api
    spec:
      containers:
      - name: transcript-api
        image: transcript-api:latest
        ports:
        - containerPort: 8080
        env:
        - name: GROQ_API_KEY
          valueFrom:
            secretKeyRef:
              name: api-secrets
              key: groq-api-key
        - name: MCP_SERVER
          value: "http://transcript-mcp:9090/mcp"
        volumeMounts:
        - name: media-storage
          mountPath: /app/shared_media
      volumes:
      - name: media-storage
        persistentVolumeClaim:
          claimName: media-pvc
```

### 🔄 **CI/CD Pipeline** (Planned)

```yaml
name: Deploy Transcript API
on:
  push:
    branches: [main]
    paths: ['src/python/transcript-api/**']

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Run tests
        run: |
          cd src/python/transcript-api
          uv sync
          uv run pytest
  
  deploy:
    needs: test
    runs-on: ubuntu-latest
    steps:
      - name: Deploy to production
        run: |
          docker build -t transcript-api:${{ github.sha }} .
          docker push registry/transcript-api:${{ github.sha }}
```

## Performance Tuning

### ⚡ **Optimization Strategies**

**FastAPI Configuration:**
```python
app = FastAPI(
    title="Transcript API",
    docs_url="/docs",
    redoc_url="/redoc",
    # Production optimizations
    debug=False,
    openapi_url="/openapi.json" if settings.DEBUG else None
)
```

**Groq Client Optimization:**
```python
# Connection pooling and timeout settings
client = Groq(
    api_key=settings.GROQ_API_KEY,
    timeout=30.0,
    max_retries=3
)
```

**Memory Management:**
```python
# Limit conversation history size
AGENT_MEMORY_SIZE = 20  # Adjust based on use case

# PixelTable optimization
pxt.configure_logging(level="WARNING")  # Reduce logging overhead
```

### 📊 **Performance Metrics**

- **Response Time**: Target < 2 seconds for chat
- **Throughput**: Handle 100+ concurrent requests
- **Memory Usage**: < 512MB per container instance
- **Token Efficiency**: Optimize prompt length and model selection

## Security Considerations

### 🔒 **API Security**

**Authentication (Planned):**
```python
from fastapi.security import HTTPBearer
security = HTTPBearer()

@app.post("/chat")
async def chat(request: UserMessageRequest, token: str = Depends(security)):
    # Validate token
    await validate_api_token(token)
```

**Input Validation:**
```python
# Pydantic models provide built-in validation
class UserMessageRequest(BaseModel):
    message: str = Field(..., min_length=1, max_length=1000)
    video_path: Optional[str] = Field(None, regex=r'^shared_media/.*\.(mp4|avi|mov)$')
```

**File Upload Security:**
```python
# Validate file types and sizes
ALLOWED_EXTENSIONS = {'.mp4', '.avi', '.mov', '.mkv'}
MAX_FILE_SIZE = 100 * 1024 * 1024  # 100MB

async def validate_upload(file: UploadFile):
    if not file.filename.endswith(tuple(ALLOWED_EXTENSIONS)):
        raise HTTPException(400, "Invalid file type")
```

### 🛡️ **Data Protection**

- **API Keys**: Stored in environment variables only
- **File Sanitization**: Clean file paths and names
- **CORS**: Configured for specific origins in production
- **Input Validation**: All inputs validated with Pydantic
- **Error Handling**: No sensitive data in error responses

## Troubleshooting

### 🚨 **Common Issues**

#### Service Won't Start
```bash
# Check dependencies
uv sync

# Verify environment variables
echo $GROQ_API_KEY

# Check logs
python src/transcript_api/api.py --port 8080

# Docker logs
docker logs transcript-api
```

#### MCP Connection Issues
```bash
# Verify MCP server is running
curl http://transcript-mcp:9090/health

# Check network connectivity
ping transcript-mcp

# Test MCP endpoint
curl http://transcript-mcp:9090/mcp
```

#### Memory/PixelTable Issues
```bash
# Clear PixelTable data
rm -rf ~/.pixeltable/

# Check disk space
df -h

# Verify write permissions
ls -la ~/.pixeltable/
```

#### Model/API Issues
```bash
# Test Groq API connectivity
curl -H "Authorization: Bearer $GROQ_API_KEY" \
     https://api.groq.com/openai/v1/models

# Check model availability
# Verify model names in configuration

# Monitor rate limits in logs
```

### 🔧 **Debug Mode**

Enable detailed logging:
```python
import logging
logging.basicConfig(level=logging.DEBUG)

# Enable Opik debug mode
import opik
opik.configure(debug=True)
```

## Contributing

### 🤝 **Development Workflow**

1. **Fork the repository**
2. **Create feature branch**: `git checkout -b feature/new-capability`
3. **Install dependencies**: `uv sync`
4. **Make changes** with proper type hints and tests
5. **Run linting**: `uv run ruff check src/`
6. **Test changes**: `uv run pytest`
7. **Submit pull request**

### 📝 **Coding Standards**

- **PEP 8**: Follow Python style guidelines
- **Type Hints**: Use comprehensive type annotations
- **Docstrings**: Document all public functions
- **Error Handling**: Explicit exception management
- **Testing**: Unit tests for new features
- **Logging**: Structured logging for debugging

### 🔄 **Release Process**

1. **Version Bump**: Update version in `pyproject.toml`
2. **Changelog**: Document changes and new features
3. **Testing**: Full test suite validation
4. **Docker Build**: Verify container builds successfully
5. **Deployment**: Deploy to staging, then production

## Roadmap

### 🎯 **Current Capabilities** (v0.1.0)
- ✅ Groq agent with multi-model support
- ✅ Video upload and processing
- ✅ MCP tool integration
- ✅ PixelTable memory system
- ✅ Opik observability
- ✅ FastAPI with CORS

### 📈 **Phase 1: Enhanced Processing** (v0.2.0 - Planned)
- 🔄 Real-time video streaming
- 🔄 Advanced clip generation
- 🔄 Multi-language support
- 🔄 Batch processing capabilities
- 🔄 Enhanced error recovery

### 🚀 **Phase 2: Scale & Performance** (v0.3.0 - Planned)
- 🔄 Horizontal scaling support
- 🔄 Caching layer implementation
- 🔄 Rate limiting and quotas
- 🔄 Advanced monitoring and alerting
- 🔄 Performance optimization

### 🎨 **Phase 3: Advanced Features** (v0.4.0 - Planned)
- 🔄 Custom model fine-tuning
- 🔄 Advanced video analytics
- 🔄 Team collaboration features
- 🔄 API versioning and backward compatibility
- 🔄 Enhanced security features

## Related Projects

- **[transcript-mcp](../transcript-mcp/)**: Model Context Protocol server for video tools
- **[transcript-consumer](../transcript-consumer/)**: Message-based transcript processing service
- **[M3.Net Modular Monolith](../../)**: Main .NET application architecture

## License

This project is part of the M3.Net ecosystem and follows the same licensing terms.

## Support

For questions, issues, or contributions:
- **Issues**: Create GitHub issues for bugs and feature requests
- **Documentation**: Check `/docs` endpoint for API documentation
- **Logs**: Review application logs for troubleshooting
- **Community**: Join discussions in project repositories

---

**Happy coding! 🚀**
