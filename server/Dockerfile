# --- Build stage ---
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

# copy both projects
COPY ./api ./api
COPY ./efscaffold ./efscaffold
COPY ./server.sln ./

RUN dotnet publish "./api/api.csproj" \
    -c Release \
    -o /app/publish \
    --runtime linux-musl-x64 \
    --self-contained true \
    /p:PublishSingleFile=true

# --- Runtime stage ---
FROM mcr.microsoft.com/dotnet/runtime-deps:9.0-alpine
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

ENTRYPOINT ["./api"]
