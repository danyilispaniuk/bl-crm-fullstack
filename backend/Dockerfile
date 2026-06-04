# Use the .NET 10 ASP.NET runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

# Use the .NET 10 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the solution file and project files first to leverage Docker cache for NuGet restore
COPY ["BL.CRM.slnx", "./"]
COPY ["src/BL.CRM.API/BL.CRM.API.csproj", "src/BL.CRM.API/"]
COPY ["src/BL.CRM.Application/BL.CRM.Application.csproj", "src/BL.CRM.Application/"]
COPY ["src/BL.CRM.Domain/BL.CRM.Domain.csproj", "src/BL.CRM.Domain/"]
COPY ["src/BL.CRM.Infrastructure/BL.CRM.Infrastructure.csproj", "src/BL.CRM.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "./src/BL.CRM.API/BL.CRM.API.csproj"

# Copy the remaining source code
COPY . .

# Build the API project
WORKDIR "/src/src/BL.CRM.API"
RUN dotnet build "./BL.CRM.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the API
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BL.CRM.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BL.CRM.API.dll"]
