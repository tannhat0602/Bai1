# Stage 1: Base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Stage 2: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Sao chép file .csproj và restore
COPY Bai1/Bai1.csproj Bai1/
WORKDIR /src/Bai1
RUN dotnet restore

# Sao chép toàn bộ mã nguồn và publish
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Stage 3: Runtime
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Bai1.dll"]
