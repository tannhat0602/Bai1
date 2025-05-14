# Giai đoạn base để chạy ứng dụng
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Giai đoạn build để build ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Sao chép file .csproj và restore
COPY *.csproj ./
RUN dotnet restore

# Sao chép toàn bộ code và publish
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Giai đoạn final để chạy app đã publish
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Bai1.dll"]
