FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /src

COPY ./Data ./Data
COPY ./TgBot ./TgBot

# Copy everything else and build
RUN dotnet publish ./TgBot/TgBot.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /src/out .
ENTRYPOINT ["dotnet", "TgBot.dll"]
