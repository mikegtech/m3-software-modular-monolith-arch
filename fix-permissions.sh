#!/bin/bash

# Fix ownership and permissions for Docker volumes
echo "Fixing permissions for Docker volumes..."

# Create directories if they don't exist
mkdir -p ./.containers/postgres_data
mkdir -p ./.containers/identity
mkdir -p ./.containers/seq_data
mkdir -p ./.containers/queue/data
mkdir -p ./.containers/queue/log
mkdir -p ./.containers/.cache
mkdir -p ./.containers/.cache/huggingface
mkdir -p ./.containers/.vscode

# Fix ownership - change from root to current user
if [ -d "./.containers" ]; then
    echo "Changing ownership of .containers directory..."
    sudo chown -R $USER:$USER ./.containers
fi

# Set proper permissions
chmod -R 755 ./.containers

echo "Permissions fixed!"
echo "You can now run: docker compose up --build"
