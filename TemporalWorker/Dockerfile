# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /out

# Run
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /out .
# Default: connect to localhost unless overridden (Docker compose passes temporal:7233)
ENV TEMPORAL__ADDRESS=localhost:7233
ENV TASK_QUEUE=housing-loans
ENTRYPOINT ["dotnet", "TemporalWorker.dll"]
