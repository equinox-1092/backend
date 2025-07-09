# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["Corebyte-platform.sln", "./"]
COPY ["Corebyte-platform/Corebyte-platform.csproj", "Corebyte-platform/"]

# Restore dependencies
RUN dotnet restore "Corebyte-platform.sln"

# Copy remaining source code
COPY . .

# Build the application
WORKDIR "/src/Corebyte-platform"
RUN dotnet build "Corebyte-platform.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "Corebyte-platform.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Copy published files
COPY --from=publish /app/publish .

# Set the entry point
ENTRYPOINT ["dotnet", "Corebyte-platform.dll"]
