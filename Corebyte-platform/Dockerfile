# Build stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Copy solution and project files
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution and project files first
COPY ["Corebyte-platform.sln", "./"]
COPY ["Corebyte-platform/Corebyte-platform.csproj", "Corebyte-platform/"]

# Restore dependencies
RUN dotnet restore "Corebyte-platform/Corebyte-platform.csproj"

# Copy the rest of your source code
COPY . .

# Build the application
WORKDIR "/src/Corebyte-platform"
RUN dotnet build "Corebyte-platform.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Corebyte-platform.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM BASE AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Corebyte-platform.dll"]
