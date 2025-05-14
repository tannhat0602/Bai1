# Stage 1: Base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Stage 2: Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project file
COPY Bai1.sln .
COPY Bai1/Bai1.csproj Bai1/

# Restore project dependencies
RUN dotnet restore Bai1/Bai1.csproj

# Copy the rest of the code
COPY . .

# Publish the project (not the solution)
RUN dotnet publish Bai1/Bai1.csproj -c Release -o /app/publish

# Stage 3: Final image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Bai1.dll"]
