# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

# Copy everything from current folder (which is already /server)
COPY . .

# Set working directory to the API project
WORKDIR /src/api

# Restore and publish
RUN dotnet restore "api.csproj"
RUN dotnet publish "api.csproj" -c Release -o /app/publish --runtime linux-musl-x64 --self-contained true /p:PublishSingleFile=true

# Runtime image
FROM mcr.microsoft.com/dotnet/runtime-deps:9.0-alpine
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["./api"]
