# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY TemporalWebApi/ .
COPY TemporalWorker/Workflows.cs ./Workflows.cs
RUN dotnet restore
RUN dotnet publish -c Release -o /out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .

# Environment variables
ENV TEMPORAL__ADDRESS=temporal:7233
ENV ASPNETCORE_URLS=http://+:5044

EXPOSE 5044
ENTRYPOINT ["dotnet", "TemporalWebApi.dll"]
