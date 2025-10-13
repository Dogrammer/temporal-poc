#!/bin/bash

# Git-based deployment script for Azure VM (POC)
# Run this on your Azure VM to deploy from GitHub repository

REPO_URL="https://github.com/Dogrammer/temporal-poc.git"
APP_DIR="~/temporal-poc"

echo "🚀 Starting Temporal POC deployment from Git..."

# Clone or update code from GitHub
if [ -d "$APP_DIR" ]; then
    echo "Updating existing code from GitHub..."
    cd "$APP_DIR"
    git pull origin master
else
    echo "Cloning repository from GitHub..."
    git clone "$REPO_URL" "$APP_DIR"
    cd "$APP_DIR"
fi

# Stop any running services
echo "Stopping existing services..."
docker compose down 2>/dev/null || true

# Start Docker services
echo "Starting Docker services (PostgreSQL, Temporal, Worker)..."
docker compose up -d --build

# Wait for services to be ready
echo "Waiting for services to start..."
sleep 15

# Check if services are running
echo "Checking services status..."
docker compose ps

# Start Web API
echo "Starting Web API..."
pkill -f "dotnet.*TemporalWebApi" 2>/dev/null || true
nohup ~/.dotnet/dotnet run --project TemporalWebApi --urls "http://0.0.0.0:5044" > webapi.log 2>&1 &

echo ""
echo "✅ Deployment complete!"
echo ""
echo "📍 Access your services at:"
echo "   Web API: http://$(curl -s ifconfig.me):5044"
echo "   Swagger: http://$(curl -s ifconfig.me):5044/swagger"
echo "   Temporal UI: http://$(curl -s ifconfig.me):8081"
echo ""
echo "📝 Test with:"
echo "   curl -X POST http://$(curl -s ifconfig.me):5044/api/workflow/start-loan \\"
echo "     -H 'Content-Type: application/json' \\"
echo "     -d '{\"loanId\": \"test123\"}'"
echo ""

