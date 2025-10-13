# Temporal POC (.NET Worker + Web API) with Docker

## What you get
- Temporal Server (PostgreSQL, Temporal, Temporal UI) via **Docker Compose**
- **.NET 8 Worker** running in Docker
- **.NET 8 Web API** to trigger workflows via HTTP endpoints
- **.NET 8 Console Client** (optional) to start a workflow

## Prereqs
- Docker / Docker Compose
- .NET 8 SDK

## 1) Start Temporal + UI + Worker
```bash
docker compose up -d --build
```
- **Temporal gRPC**: `localhost:7233`
- **Temporal UI**: http://localhost:8081
- **PostgreSQL**: `localhost:5432`

## 2) Start a workflow using the Web API

### Run the Web API
In a terminal:
```bash
dotnet run --project TemporalWebApi --urls "http://localhost:5000"
```

### API Endpoints

#### Start a new loan workflow
```bash
curl -X POST http://localhost:5000/api/workflow/start-loan \
  -H "Content-Type: application/json" \
  -d '{"loanId": "test123"}'
```

Response:
```json
{
  "workflowId": "kredit-test123",
  "loanId": "test123",
  "status": "started"
}
```

#### Get workflow status
```bash
curl http://localhost:5000/api/workflow/status/kredit-test123
```

#### Get workflow result
```bash
curl http://localhost:5000/api/workflow/result/kredit-test123
```

### Swagger UI
Access the API documentation at: http://localhost:5000/swagger

## 3) Alternative: Start a workflow from the console client
```bash
dotnet run --project TemporalClientApp
```
You should see:
```
Started workflow kredit-<guid>
Result: Hello, Kredit <guid>!
```

## Architecture

- **Web API**: Exposes HTTP endpoints to trigger and monitor workflows
- **Worker**: Executes workflow and activity code (runs in Docker)
- **Temporal Server**: Orchestrates workflows and manages state
- **PostgreSQL**: Stores workflow history and state
- **Temporal UI**: Web interface to monitor workflows

## Notes
- Worker connects to Temporal using env var `TEMPORAL__ADDRESS` (defaults to `localhost:7233`)
  - In Docker, we pass `temporal:7233` so it can reach the server on the internal network
- Task queue is `housing-loans` (override with `TASK_QUEUE` env var)
- Web API connects to Temporal at `localhost:7233` (configurable in appsettings.json)

## Clean up
```bash
docker compose down -v
```
