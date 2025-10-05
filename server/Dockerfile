FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

# Copy all project files first
COPY . .
WORKDIR /src/api

# Restore now that all referenced projects exist
RUN dotnet restore "api.csproj"

# Publish
RUN dotnet publish "api.csproj" \
    -c Release \
    -o /app/publish \
    --runtime linux-musl-x64 \
    --self-contained true \
    /p:PublishSingleFile=true

# Runtime image
FROM mcr.microsoft.com/dotnet/runtime-deps:9.0-alpine
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://localhost:8080/
EXPOSE 8080
ENTRYPOINT ["./api"]
