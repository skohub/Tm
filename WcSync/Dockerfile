FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

COPY ./WcSync .

# Copy everything else and build
RUN dotnet build ./WcSync.Cli/WcSync.Cli.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "WcSync.Cli.dll"]
