# Stage 1: Build and publish stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the solution file and project files first for optimal caching
COPY ["UniConnect.slnx", "./"]
COPY ["src/UniConnect.Api/UniConnect.Api.csproj", "src/UniConnect.Api/"]
COPY ["src/UniConnect.Application/UniConnect.Application.csproj", "src/UniConnect.Application/"]
COPY ["src/UniConnect.Domain/UniConnect.Domain.csproj", "src/UniConnect.Domain/"]
COPY ["src/UniConnect.Infrastructure/UniConnect.Infrastructure.csproj", "src/UniConnect.Infrastructure/"]

# Restore NuGet dependencies for the solution
RUN dotnet restore "UniConnect.slnx"

# Copy the rest of the source code
COPY . .

# Build and publish the API project
WORKDIR "/src/src/UniConnect.Api"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Copy the published output from the build stage
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "UniConnect.Api.dll"]