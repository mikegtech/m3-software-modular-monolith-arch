FROM python:3.12-slim

COPY --from=ghcr.io/astral-sh/uv:latest /uv /uvx /bin/

COPY . /app
WORKDIR /app

RUN uv sync --frozen --no-cache
RUN uv pip install -e .

ENTRYPOINT ["uvicorn", "transcripts.app:app", "--host", "0.0.0.0", "--port", "8080", "--lifespan", "on"]