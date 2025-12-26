# Pull .NET 8 ASP.NET Core runtime image (lightweight, no SDK)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Pull .NET 8 SDK image (includes compiler and build tools)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# Copy .csproj first for Docker layer caching (if unchanged, NuGet restore is cached)
COPY ["TaskService.csproj", "./"]
# Restore NuGet packages defined in .csproj
RUN dotnet restore "TaskService.csproj"
# Copy all source code files
COPY . .
# Compile the application in Release mode, output to /app/build
RUN dotnet build "TaskService.csproj" -c Release -o /app/build

# Create optimized publish version (removes unnecessary files, ready for deployment)
FROM build AS publish
RUN dotnet publish "TaskService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use lightweight runtime image and copy only published application files
FROM base AS final
WORKDIR /app
# Copy published files from publish stage (no SDK, no source code)
COPY --from=publish /app/publish .

# Configure health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
  CMD curl --fail http://localhost:8080/health || exit 1

# Start the application when container runs
ENTRYPOINT ["dotnet", "TaskService.dll"]