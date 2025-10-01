# server/Dockerfile

FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

COPY server/server.sln ./
COPY server/api/api.csproj ./api/

RUN dotnet restore "server.sln"

COPY server/ ./

RUN dotnet publish "api/api.csproj" \
    -c Release \
    -o /app/publish \
    --runtime linux-musl-x64 \
    --self-contained true \
    /p:PublishSingleFile=true

FROM mcr.microsoft.com/dotnet/runtime-deps:9.0-alpine
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080
ENTRYPOINT ["./api"]
