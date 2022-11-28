FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
WORKDIR /source

COPY ./Data ./Data
COPY ./TgBot ./TgBot

# Copy everything else and build
RUN dotnet publish TgBot/TgBot.csproj -c Release -o /out -r linux-musl-x64 --self-contained true /p:PublishReadyToRun=true

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime-deps:7.0-alpine-amd64
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["./TgBot"]
