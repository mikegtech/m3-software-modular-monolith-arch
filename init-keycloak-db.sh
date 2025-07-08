#!/bin/bash

# Wait for PostgreSQL to be ready and create Keycloak database
echo "Waiting for PostgreSQL to be ready..."

# Wait for PostgreSQL to start
until docker exec m3.net.postgres pg_isready -U postgres; do
  echo "PostgreSQL is not ready yet..."
  sleep 2
done

echo "PostgreSQL is ready! Creating Keycloak database..."

# Create the keycloak database if it doesn't exist
docker exec m3.net.postgres psql -U postgres -c "SELECT 1 FROM pg_database WHERE datname='keycloak'" | grep -q 1 || docker exec m3.net.postgres createdb -U postgres keycloak

echo "Keycloak database setup complete!"
