# Transcript MCP Server

A sophisticated Model Context Protocol (MCP) server that provides advanced video processing and search capabilities using AI-powered semantic analysis. This server enables intelligent video content understanding through multi-modal search across speech, visual frames, and captions.

## Overview

The Transcript MCP Server is a FastMCP-based service that combines:
- **Video Processing Pipeline**: Automated video ingestion with frame extraction and audio chunking
- **Multi-Modal Search**: Semantic search across speech transcripts, visual frames, and AI-generated captions
- **AI Integration**: OpenAI Whisper for transcription, GPT models for captioning, and CLIP for visual similarity
- **Persistent Storage**: PixelTable-based video indexing with vector embeddings
- **MCP Protocol**: Standard interface for AI agents to interact with video content

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    AI Agent / Client                        │
├─────────────────────────────────────────────────────────────┤
│  transcript-api, Claude Desktop, etc.                      │
└─────────────────────────┬───────────────────────────────────┘
                          │ MCP Protocol
                          ▼
┌─────────────────────────────────────────────────────────────┐
│                  Transcript MCP Server                      │
├─────────────────────────────────────────────────────────────┤
│  FastMCP + Tools + Prompts + Resources                     │
└─────────────────────────┬───────────────────────────────────┘
                          │
          ┌───────────────┴───────────────┐
          │                               │
          ▼                               ▼
┌──────────────────────┐         ┌─────────────────────┐
│  Video Processing    │         │   Search Engine     │
├──────────────────────┤         ├─────────────────────┤
│ • Frame Extraction   │         │ • Speech Search     │
│ • Audio Chunking     │         │ • Image Search      │
│ • Transcription      │         │ • Caption Search    │
│ • Caption Generation │         │ • Semantic Matching │
└──────────────────────┘         └─────────────────────┘
          │                               │
          ▼                               ▼
┌─────────────────────────────────────────────────────────────┐
│                    PixelTable Storage                       │
├─────────────────────────────────────────────────────────────┤
│  Video Index • Embeddings • Metadata • Search Indexes      │
└─────────────────────────┬───────────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────────────┐
│                    External Services                        │
├─────────────────────────────────────────────────────────────┤
│  OpenAI API • MoviePy • FFmpeg • Opik/Comet ML             │
└─────────────────────────────────────────────────────────────┘
```

## Features

### 🎥 **Video Processing**
- **Automated Ingestion**: Process video files with frame extraction and audio separation
- **Frame Analysis**: Extract frames at configurable intervals with AI-generated captions
- **Audio Transcription**: Whisper-powered speech-to-text with chunking and overlap
- **Smart Chunking**: Configurable audio chunks with temporal overlap for accuracy
- **Persistent Indexing**: PixelTable-based storage with vector embeddings

### 🔍 **Multi-Modal Search**
- **Speech Search**: Semantic search across transcribed audio content
- **Visual Search**: Image similarity search using CLIP embeddings
- **Caption Search**: Text-based search through AI-generated frame descriptions
- **Question Answering**: Natural language queries against video content
- **Clip Extraction**: Automated video clip generation from search results

### 🛠️ **MCP Tools**
- **`process_video`**: Ingest and index video files for search
- **`get_video_clip_from_user_query`**: Extract clips based on text queries
- **`get_video_clip_from_image`**: Find similar video segments from image input
- **`ask_question_about_video`**: Answer questions about video content

### 📝 **MCP Prompts**
- **Routing Prompts**: Determine when to use video tools
- **Tool Use Prompts**: Guide tool selection and parameter setting
- **General Prompts**: Conversational AI with Kubrick-inspired personality

### 📊 **MCP Resources**
- **Video Registry**: List and manage indexed video content
- **Table Metadata**: Access to video processing statistics and indexes

## Technology Stack

### Core Dependencies
- **Python 3.12+**: Modern Python with comprehensive type hints
- **FastMCP**: Model Context Protocol server framework
- **PixelTable**: Vector database for video content and embeddings
- **OpenAI**: GPT models for transcription and captioning
- **MoviePy**: Video processing and manipulation
- **Sentence Transformers**: Text embeddings for semantic search
- **Transformers/CLIP**: Multi-modal embeddings for image-text similarity

### AI Models
- **Audio Transcription**: `gpt-4o-mini-transcribe` (Whisper)
- **Image Captioning**: `gpt-4o-mini`
- **Text Embeddings**: `text-embedding-3-small`
- **Visual Embeddings**: `openai/clip-vit-base-patch32`

### Infrastructure
- **FFmpeg**: Video/audio processing backend
- **Loguru**: Structured logging and debugging
- **Opik**: AI application observability and tracking
- **Pydantic**: Configuration management and data validation

## Quick Start

### Prerequisites
- Python 3.12 or higher
- FFmpeg installed on system
- OpenAI API key
- Optional: Opik API key for observability

### Installation

1. **Clone and navigate to the project**:
   ```bash
   cd src/python/transcript-mcp
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
OPENAI_API_KEY=your_openai_api_key_here

# Optional - for observability
OPIK_API_KEY=your_opik_api_key_here
OPIK_WORKSPACE=default
OPIK_PROJECT=transcript-mcp
```

### Running the Server

#### Development Mode
```bash
# With uv
uv run python src/kubrick_mcp/server.py --port 9090 --host 0.0.0.0

# Direct execution
python src/kubrick_mcp/server.py --port 9090 --host 0.0.0.0 --transport streamable-http
```

#### Production Mode
```bash
# With production settings
python src/kubrick_mcp/server.py --port 9090 --host 0.0.0.0 --transport streamable-http
```

#### Docker
```bash
# Build the image
docker build -t transcript-mcp .

# Run the container
docker run -p 9090:9090 \
  -e OPENAI_API_KEY=your_key_here \
  -e OPIK_API_KEY=your_opik_key \
  -v $(pwd)/shared_media:/app/shared_media \
  -v $(pwd)/video_cache:/app/video_cache \
  transcript-mcp
```

## MCP Tools Reference

### 🎬 **process_video**
Process and index a video file for search capabilities.

**Parameters:**
- `video_path` (str): Path to the video file to process

**Returns:**
- `str`: Success message or processing status

**Example:**
```python
# Via MCP client
result = await mcp_client.call_tool("process_video", {
    "video_path": "shared_media/my_video.mp4"
})
```

### 🔍 **get_video_clip_from_user_query**
Extract a video clip based on semantic search of the query.

**Parameters:**
- `video_path` (str): Path to the indexed video file
- `user_query` (str): Natural language search query

**Returns:**
- `Dict[str, str]`: `{"clip_path": "path/to/extracted/clip.mp4"}`

**Example:**
```python
result = await mcp_client.call_tool("get_video_clip_from_user_query", {
    "video_path": "shared_media/sports_video.mp4",
    "user_query": "player scoring a goal"
})
# Returns: {"clip_path": "shared_media/uuid-clip.mp4"}
```

### 🖼️ **get_video_clip_from_image**
Find and extract video clips similar to a provided image.

**Parameters:**
- `video_path` (str): Path to the indexed video file
- `user_image` (str): Base64-encoded image for similarity search

**Returns:**
- `Dict[str, str]`: `{"clip_path": "path/to/extracted/clip.mp4"}`

**Example:**
```python
result = await mcp_client.call_tool("get_video_clip_from_image", {
    "video_path": "shared_media/match_video.mp4",
    "user_image": "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQAAAQ..."
})
```

### ❓ **ask_question_about_video**
Answer questions about video content using transcript analysis.

**Parameters:**
- `video_path` (str): Path to the indexed video file  
- `user_query` (str): Question about the video content

**Returns:**
- `Dict[str, str]`: `{"answer": "aggregated_relevant_captions"}`

**Example:**
```python
result = await mcp_client.call_tool("ask_question_about_video", {
    "video_path": "shared_media/documentary.mp4",
    "user_query": "What is discussed about climate change?"
})
# Returns: {"answer": "The video discusses rising temperatures..."}
```

## MCP Prompts Reference

### 🧭 **routing_system_prompt**
Determines when video tools should be used based on user queries.

**Usage in AI Agents:**
```python
routing_prompt = await mcp_client.get_prompt("routing_system_prompt")
# Use for routing decisions in conversational AI
```

### 🔧 **tool_use_system_prompt** 
Guides AI agents in selecting appropriate video tools and parameters.

**Context Variables:**
- `{is_image_provided}`: Boolean indicating if user provided an image

### 💬 **general_system_prompt**
Provides personality and context for the "Transcript" AI assistant.

**Personality Features:**
- Inspired by Stanley Kubrick (director themes)
- Movie references and film knowledge
- Technical video processing expertise
- HAL 9000 connection for AI authenticity

## Video Processing Pipeline

### 📹 **Ingestion Workflow**

1. **Video Input**: Accept video file path for processing
2. **Frame Extraction**: Extract frames at configured FPS intervals
3. **Audio Separation**: Extract audio track and chunk into segments  
4. **Transcription**: Use Whisper to transcribe audio chunks
5. **Caption Generation**: Generate AI descriptions for extracted frames
6. **Embedding Creation**: Create vector embeddings for search
7. **Index Storage**: Store in PixelTable with metadata
8. **Registry Update**: Update video index registry

### 🔍 **Search Workflow**

1. **Query Input**: Receive text query, image, or question
2. **Embedding Generation**: Create query embeddings
3. **Similarity Search**: Compare against stored embeddings
4. **Result Ranking**: Score and rank matching segments
5. **Clip Extraction**: Generate video clips from top matches
6. **Response Formatting**: Return structured results

### ⚙️ **Processing Configuration**

```python
# Frame extraction settings
SPLIT_FRAMES_COUNT = 45  # FPS for frame extraction
DELTA_SECONDS_FRAME_INTERVAL = 5.0  # Clip padding around frames

# Audio processing settings  
AUDIO_CHUNK_LENGTH = 10  # Seconds per chunk
AUDIO_OVERLAP_SECONDS = 1  # Overlap between chunks
AUDIO_MIN_CHUNK_DURATION_SECONDS = 1  # Minimum chunk size

# Image processing settings
IMAGE_RESIZE_WIDTH = 1024
IMAGE_RESIZE_HEIGHT = 768

# Search configuration
VIDEO_CLIP_SPEECH_SEARCH_TOP_K = 1  # Top speech results
VIDEO_CLIP_CAPTION_SEARCH_TOP_K = 1  # Top caption results  
VIDEO_CLIP_IMAGE_SEARCH_TOP_K = 1  # Top image results
QUESTION_ANSWER_TOP_K = 3  # Top Q&A results
```

## Configuration Reference

### Environment Variables

| Variable | Description | Default | Required |
|----------|-------------|---------|----------|
| `OPENAI_API_KEY` | OpenAI API authentication key | - | ✅ |
| `OPIK_API_KEY` | Opik observability API key | - | ❌ |
| `OPIK_WORKSPACE` | Opik workspace name | `default` | ❌ |
| `OPIK_PROJECT` | Opik project name | `transcript-mcp` | ❌ |

### Model Configuration

| Setting | Description | Default |
|---------|-------------|---------|
| `AUDIO_TRANSCRIPT_MODEL` | Whisper model for transcription | `gpt-4o-mini-transcribe` |
| `IMAGE_CAPTION_MODEL` | GPT model for image captioning | `gpt-4o-mini` |
| `TRANSCRIPT_SIMILARITY_EMBD_MODEL` | Text embedding model | `text-embedding-3-small` |
| `IMAGE_SIMILARITY_EMBD_MODEL` | Visual embedding model | `openai/clip-vit-base-patch32` |
| `CAPTION_SIMILARITY_EMBD_MODEL` | Caption embedding model | `openai/clip-vit-base-patch32` |

### Processing Parameters

| Setting | Description | Default |
|---------|-------------|---------|
| `SPLIT_FRAMES_COUNT` | Frame extraction FPS | `45` |
| `AUDIO_CHUNK_LENGTH` | Audio chunk duration (seconds) | `10` |
| `AUDIO_OVERLAP_SECONDS` | Chunk overlap duration | `1` |
| `AUDIO_MIN_CHUNK_DURATION_SECONDS` | Minimum chunk size | `1` |
| `IMAGE_RESIZE_WIDTH` | Frame resize width | `1024` |
| `IMAGE_RESIZE_HEIGHT` | Frame resize height | `768` |
| `DELTA_SECONDS_FRAME_INTERVAL` | Clip padding around frames | `5.0` |

## Storage & Data Management

### 📊 **PixelTable Schema**

**Video Table Structure:**
```python
video_table = pxt.create_table('video_name', {
    'video': pxt.Video,           # Original video file
    'audio': pxt.Audio,           # Extracted audio track
})

frames_view = pxt.create_view('frames', video_table, {
    'frame': FrameIterator(video_table.video, fps=45),
    'pos_msec': frames_view.frame.pos,
    'resized_frame': resize_image(frames_view.frame, width=1024, height=768),
    'frame_caption': vision.chat_completions(
        model='gpt-4o-mini',
        messages=[{'role': 'user', 'content': 'Describe what is happening in the image'}],
        images=[frames_view.resized_frame]
    ),
    'caption_embedding': clip.embed_text(frames_view.frame_caption)
})

audio_chunks_view = pxt.create_view('audio_chunks', video_table, {
    'chunk': AudioSplitter(video_table.audio, chunk_size=10, overlap=1),
    'start_time_sec': audio_chunks_view.chunk.start_time,
    'end_time_sec': audio_chunks_view.chunk.end_time,
    'chunk_text': openai.transcriptions(
        model='gpt-4o-mini-transcribe',
        audio=audio_chunks_view.chunk
    ),
    'text_embedding': embeddings(
        model='text-embedding-3-small', 
        input=audio_chunks_view.chunk_text
    )
})
```

### 🗂️ **Registry Management**

**Cached Table Registry:**
```python
# Video index registration
registry.register_table(video_name, CachedTable(
    video_name=video_name,
    video_cache=cache_path,
    video_table=video_table,
    frames_view=frames_view, 
    audio_chunks_view=audio_chunks_view
))

# Video index retrieval
cached_table = registry.get_table(video_name)
```

## Integration Examples

### 🤖 **AI Agent Integration**

```python
from fastmcp import Client

# Connect to MCP server
mcp_client = Client("http://transcript-mcp:9090/mcp")

async def process_video_request(video_path: str, user_query: str):
    """Example AI agent workflow"""
    
    # 1. Ensure video is processed
    await mcp_client.call_tool("process_video", {
        "video_path": video_path
    })
    
    # 2. Get relevant video clip
    clip_result = await mcp_client.call_tool("get_video_clip_from_user_query", {
        "video_path": video_path,
        "user_query": user_query
    })
    
    # 3. Get additional context
    context_result = await mcp_client.call_tool("ask_question_about_video", {
        "video_path": video_path,
        "user_query": f"Provide context about: {user_query}"
    })
    
    return {
        "clip_path": clip_result["clip_path"],
        "context": context_result["answer"]
    }
```

### 🔧 **Custom Tool Development**

```python
# Adding new MCP tools
def add_custom_tool(mcp: FastMCP):
    @mcp.add_tool(
        name="analyze_video_sentiment",
        description="Analyze emotional sentiment in video content",
        tags={"video", "sentiment", "analysis"}
    )
    def analyze_video_sentiment(video_path: str) -> Dict[str, Any]:
        search_engine = VideoSearchEngine(video_path)
        # Custom sentiment analysis implementation
        return {"sentiment": "positive", "confidence": 0.85}
```

## Observability

### 📊 **Opik Integration**

**Comprehensive tracking of:**
- **Tool Executions**: All MCP tool calls with parameters and results
- **Video Processing**: Ingestion pipeline performance and errors
- **Search Operations**: Query performance and result quality
- **Model Usage**: API calls, token usage, and response times

**Usage:**
```python
import opik

# Automatic tracking via decorators
@opik.track(name="video-processing")
def process_video(video_path: str) -> str:
    # Automatically tracked with Opik
    pass
```

### 🚨 **Logging**

**Structured logging with Loguru:**
```python
logger.bind(name="VideoProcessor").info(
    "Processing video", 
    video_path=video_path,
    frame_count=total_frames,
    duration_seconds=video_duration
)
```

### 📈 **Performance Metrics**

- **Processing Speed**: Video ingestion time per minute of content
- **Search Latency**: Query response times across modalities
- **Storage Efficiency**: Index size vs. video duration ratios
- **Model Performance**: Transcription accuracy and caption quality

## Development

### 📁 **Project Structure**
```
transcript-mcp/
├── src/
│   └── kubrick_mcp/
│       ├── video/                        # Video processing modules
│       │   ├── ingestion/               # Video ingestion pipeline
│       │   │   ├── video_processor.py   # Main video processing logic
│       │   │   ├── models.py           # Data models and schemas
│       │   │   ├── registry.py         # Video index registry
│       │   │   ├── tools.py            # Processing utilities
│       │   │   ├── functions.py        # PixelTable functions
│       │   │   └── constants.py        # Processing constants
│       │   └── video_search_engine.py   # Multi-modal search engine
│       ├── server.py                    # FastMCP server
│       ├── tools.py                     # MCP tool implementations
│       ├── prompts.py                   # MCP prompt management
│       ├── resources.py                 # MCP resource providers
│       ├── config.py                    # Configuration management
│       ├── opik_utils.py               # Observability utilities
│       └── __init__.py
├── shared_media/                        # Generated clips and media
├── .env.example                         # Environment template
├── Dockerfile                           # Container configuration
├── pyproject.toml                       # Project dependencies
└── README.md                           # This file
```

### 🔧 **Code Style**

- **Type Hints**: Comprehensive type annotations throughout
- **Async/Await**: Async patterns where beneficial
- **Pydantic Models**: Data validation and configuration
- **Modular Design**: Clear separation of concerns
- **Error Handling**: Comprehensive exception management

### 🧪 **Adding New Features**

1. **New MCP Tools**: Add to `tools.py` and register in `server.py`
2. **Search Capabilities**: Extend `VideoSearchEngine` class
3. **Processing Features**: Modify `VideoProcessor` ingestion pipeline
4. **Storage Schema**: Update PixelTable models in `models.py`
5. **Configuration**: Add settings to `config.py`

## Testing

### 🧪 **Testing Strategy** (Planned)

```bash
# Unit tests
uv run pytest tests/unit/

# Integration tests (requires OpenAI API)
uv run pytest tests/integration/

# MCP protocol tests
uv run pytest tests/mcp/

# End-to-end video processing tests
uv run pytest tests/e2e/

# All tests
uv run pytest
```

### 📝 **Test Categories** (Planned)

- **Unit Tests**: Individual component testing (search, processing, tools)
- **Integration Tests**: OpenAI API integration, PixelTable operations
- **MCP Tests**: Protocol compliance, tool registration, prompt delivery
- **Performance Tests**: Video processing speed, search latency, memory usage
- **Quality Tests**: Transcription accuracy, caption quality, search relevance

### 🎯 **Test Examples** (Planned)

```python
async def test_video_processing():
    """Test complete video processing pipeline"""
    processor = VideoProcessor()
    result = processor.setup_table("test_video.mp4")
    assert result is True
    
    # Verify index creation
    cached_table = registry.get_table("test_video.mp4")
    assert cached_table is not None
    assert len(cached_table.frames_view.collect()) > 0

async def test_speech_search():
    """Test speech-based video search"""
    search_engine = VideoSearchEngine("indexed_video.mp4")
    results = search_engine.search_by_speech("goal scored", top_k=1)
    
    assert len(results) == 1
    assert results[0]["similarity"] > 0.5
    assert "start_time" in results[0]
    assert "end_time" in results[0]

async def test_mcp_tool_integration():
    """Test MCP tool functionality"""
    result = await mcp_client.call_tool("get_video_clip_from_user_query", {
        "video_path": "test_video.mp4",
        "user_query": "player celebration"
    })
    
    assert "clip_path" in result
    assert result["clip_path"].endswith(".mp4")
    assert Path(result["clip_path"]).exists()
```

## Deployment

### 🐳 **Docker Deployment**

**Production container:**
```bash
# Build optimized image
docker build -t transcript-mcp:latest .

# Run with production settings
docker run -d \
  --name transcript-mcp \
  -p 9090:9090 \
  -e OPENAI_API_KEY=your_key \
  -e OPIK_API_KEY=your_opik_key \
  -v /path/to/media:/app/shared_media \
  -v /path/to/cache:/app/video_cache \
  --restart unless-stopped \
  transcript-mcp:latest
```

### ☸️ **Kubernetes Deployment** (Planned)

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: transcript-mcp
spec:
  replicas: 2
  selector:
    matchLabels:
      app: transcript-mcp
  template:
    metadata:
      labels:
        app: transcript-mcp
    spec:
      containers:
      - name: transcript-mcp
        image: transcript-mcp:latest
        ports:
        - containerPort: 9090
        env:
        - name: OPENAI_API_KEY
          valueFrom:
            secretKeyRef:
              name: api-secrets
              key: openai-api-key
        volumeMounts:
        - name: media-storage
          mountPath: /app/shared_media
        - name: cache-storage
          mountPath: /app/video_cache
        resources:
          requests:
            memory: "2Gi"
            cpu: "1000m"
          limits:
            memory: "4Gi" 
            cpu: "2000m"
      volumes:
      - name: media-storage
        persistentVolumeClaim:
          claimName: media-pvc
      - name: cache-storage
        persistentVolumeClaim:
          claimName: cache-pvc
```

### 🔄 **Docker Compose** (Planned)

```yaml
version: '3.8'
services:
  transcript-mcp:
    build: .
    ports:
      - "9090:9090"
    environment:
      - OPENAI_API_KEY=${OPENAI_API_KEY}
      - OPIK_API_KEY=${OPIK_API_KEY}
    volumes:
      - ./shared_media:/app/shared_media
      - ./video_cache:/app/video_cache
    restart: unless-stopped
    
  transcript-api:
    depends_on:
      - transcript-mcp
    environment:
      - MCP_SERVER=http://transcript-mcp:9090/mcp
```

## Performance Tuning

### ⚡ **Optimization Strategies**

**Video Processing:**
```python
# Optimize frame extraction
SPLIT_FRAMES_COUNT = 30  # Reduce FPS for faster processing
IMAGE_RESIZE_WIDTH = 512  # Smaller images for faster embedding

# Optimize audio chunking  
AUDIO_CHUNK_LENGTH = 15  # Larger chunks for better context
AUDIO_OVERLAP_SECONDS = 2  # More overlap for accuracy
```

**Search Performance:**
```python
# Limit search scope
VIDEO_CLIP_SEARCH_TOP_K = 3  # Reduce result count
QUESTION_ANSWER_TOP_K = 5   # Balance between speed and accuracy

# PixelTable optimization
pxt.configure_logging(level="WARNING")  # Reduce logging overhead
```

**Memory Management:**
```python
# Process videos in batches
MAX_CONCURRENT_VIDEOS = 2  # Limit parallel processing
CACHE_CLEANUP_THRESHOLD = 100  # Clean old indexes
```

### 📊 **Performance Targets**

- **Processing Speed**: 10x faster than real-time (1 min video in 6 seconds)
- **Search Latency**: < 500ms for text queries, < 1s for image queries
- **Memory Usage**: < 2GB per video index
- **Throughput**: Process 10+ videos concurrently

## Security Considerations

### 🔒 **API Security**

**Authentication:**
```python
# Environment-based API key management
OPENAI_API_KEY = os.getenv("OPENAI_API_KEY")
if not OPENAI_API_KEY:
    raise ValueError("OpenAI API key required")
```

**Input Validation:**
```python
# Pydantic models provide built-in validation
class VideoPath(BaseModel):
    path: str = Field(..., regex=r'^[a-zA-Z0-9_/.-]+\.(mp4|avi|mov|mkv)$')
    
def validate_video_file(video_path: str) -> bool:
    """Validate video file exists and is safe"""
    path = Path(video_path)
    return path.exists() and path.suffix.lower() in ALLOWED_EXTENSIONS
```

**File System Security:**
```python
# Sanitize file paths
def sanitize_path(path: str) -> str:
    """Remove dangerous path components"""
    return str(Path(path).resolve()).replace("..", "")

# Restrict file access to designated directories
ALLOWED_DIRECTORIES = ["/app/shared_media", "/app/video_cache"]
```

### 🛡️ **Data Protection**

- **API Keys**: Environment variables only, never in code
- **File Isolation**: Restricted file system access
- **Input Sanitization**: All inputs validated and sanitized
- **Error Handling**: No sensitive information in error messages
- **Audit Logging**: All operations logged for security review

## Troubleshooting

### 🚨 **Common Issues**

#### Server Won't Start
```bash
# Check dependencies
uv sync

# Verify FFmpeg installation
ffmpeg -version

# Check environment variables
echo $OPENAI_API_KEY

# Check logs
python src/kubrick_mcp/server.py --port 9090

# Docker logs
docker logs transcript-mcp
```

#### Video Processing Fails
```bash
# Verify video file format
ffprobe video_file.mp4

# Check file permissions
ls -la shared_media/

# Verify OpenAI API key
curl -H "Authorization: Bearer $OPENAI_API_KEY" \
     https://api.openai.com/v1/models

# Check PixelTable storage
ls -la ~/.pixeltable/
```

#### Search Returns No Results
```bash
# Verify video is indexed
python -c "
import kubrick_mcp.video.ingestion.registry as registry
print(registry.list_tables())
"

# Check embeddings
python -c "
from kubrick_mcp.video.video_search_engine import VideoSearchEngine
engine = VideoSearchEngine('video.mp4')
results = engine.search_by_speech('test query', 5)
print(results)
"

# Verify search engine setup
python -c "
from kubrick_mcp.video.video_search_engine import VideoSearchEngine
try:
    engine = VideoSearchEngine('nonexistent.mp4')
except ValueError as e:
    print(f'Expected error: {e}')
"
```

#### MCP Connection Issues
```bash
# Test MCP server
curl http://localhost:9090/health

# Verify MCP protocol
curl -X POST http://localhost:9090/mcp \
     -H "Content-Type: application/json" \
     -d '{"method": "tools/list"}'

# Check FastMCP status
netstat -tulpn | grep 9090
```

### 🔧 **Debug Mode**

Enable detailed logging:
```python
import logging
logging.basicConfig(level=logging.DEBUG)

# Enable PixelTable debug mode
import pixeltable as pxt
pxt.configure_logging(level="DEBUG")

# Enable Opik debug mode
import opik
opik.configure(debug=True)
```

## Contributing

### 🤝 **Development Workflow**

1. **Fork the repository**
2. **Create feature branch**: `git checkout -b feature/new-video-tool`
3. **Install dependencies**: `uv sync`
4. **Make changes** with proper type hints and tests
5. **Run linting**: `uv run ruff check src/`
6. **Test changes**: `uv run pytest`
7. **Update documentation**: README and docstrings
8. **Submit pull request**

### 📝 **Coding Standards**

- **PEP 8**: Follow Python style guidelines
- **Type Hints**: Use comprehensive type annotations
- **Docstrings**: Document all public functions and classes
- **Error Handling**: Explicit exception management
- **Testing**: Unit tests for new features
- **Logging**: Structured logging for debugging
- **MCP Compliance**: Follow MCP protocol specifications

### 🔄 **Release Process**

1. **Version Bump**: Update version in `pyproject.toml`
2. **Changelog**: Document changes and new features
3. **Testing**: Full test suite validation including MCP protocol tests
4. **Docker Build**: Verify container builds and runs successfully
5. **Documentation**: Update README and API documentation
6. **Deployment**: Deploy to staging, then production

## Roadmap

### 🎯 **Current Capabilities** (v0.1.0)
- ✅ FastMCP server with video tools
- ✅ Multi-modal video search (speech, image, caption)
- ✅ PixelTable-based video indexing
- ✅ OpenAI integration for transcription and captioning
- ✅ Clip extraction and generation
- ✅ Opik observability integration

### 📈 **Phase 1: Enhanced Processing** (v0.2.0 - Planned)
- 🔄 Real-time video stream processing
- 🔄 Advanced video analytics (object detection, scene segmentation)
- 🔄 Multi-language support for transcription
- 🔄 Custom model fine-tuning capabilities
- 🔄 Batch video processing optimization

### 🚀 **Phase 2: Scale & Performance** (v0.3.0 - Planned)
- 🔄 Horizontal scaling with distributed processing
- 🔄 Caching layer for frequent queries
- 🔄 GPU acceleration for video processing
- 🔄 Advanced search algorithms and ranking
- 🔄 Performance monitoring and auto-scaling

### 🎨 **Phase 3: Advanced Features** (v0.4.0 - Planned)
- 🔄 Live video stream analysis
- 🔄 Advanced NLP for video content understanding
- 🔄 Custom embedding models for domain-specific content
- 🔄 API versioning and backward compatibility
- 🔄 Advanced security and access control

## Related Projects

- **[transcript-api](../transcript-api/)**: AI agent interface that consumes this MCP server
- **[transcript-consumer](../transcript-consumer/)**: Message-based batch transcript processing
- **[M3.Net Modular Monolith](../../)**: Main .NET application architecture

## License

This project is part of the M3.Net ecosystem and follows the same licensing terms.

## Support

For questions, issues, or contributions:
- **Issues**: Create GitHub issues for bugs and feature requests
- **Documentation**: Check MCP protocol documentation for integration details
- **Logs**: Review application logs for troubleshooting and debugging
- **Community**: Join discussions in project repositories

---

**"I'm sorry, Dave. I'm afraid I can't do that... but I can help you find that exact video clip!" 🎬**
