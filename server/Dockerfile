# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

COPY server/api/api.csproj ./server/api/
RUN dotnet restore ./server/api/api.csproj

COPY . .
RUN dotnet publish ./server/api/api.csproj \
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

# ✅ Run the self-contained binary directly
ENTRYPOINT ["./api"]
