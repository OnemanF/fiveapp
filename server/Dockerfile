# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

# Copy and restore
COPY api/api.csproj ./api/
RUN dotnet restore ./api/api.csproj

# Copy all and publish
COPY . .
RUN dotnet publish ./api/api.csproj \
    -c Release \
    -o /app/publish \
    --self-contained true \
    --runtime linux-musl-x64 \
    /p:PublishSingleFile=true

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/runtime-deps:9.0-alpine
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

# Must match output binary name (usually 'api')
ENTRYPOINT ["./api"]
