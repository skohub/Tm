FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /src

COPY ./Data ./Data
COPY ./TgBot ./TgBot

# Copy everything else and build
RUN dotnet build ./TgBot/TgBot.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:7.0
WORKDIR /app
COPY --from=build-env /src/out .
ENTRYPOINT ["dotnet", "TgBot.dll"]
