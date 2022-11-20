FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY WcSync/WcSync.Cli/*.csproj WcSync/WcSync.Cli/
COPY WcSync/WcSync.Model/*.csproj WcSync/WcSync.Model/
COPY WcSync/WcSync.Sync/*.csproj WcSync/WcSync.Sync/
COPY WcSync/WcSync.Wc/*.csproj WcSync/WcSync.Wc/
COPY Data/*.csproj Data/
RUN dotnet restore WcSync/WcSync.Cli/WcSync.Cli.csproj

# copy and build app and libraries
COPY WcSync/WcSync.Cli/ WcSync/WcSync.Cli/
COPY WcSync/WcSync.Model/ WcSync/WcSync.Model/
COPY WcSync/WcSync.Sync/ WcSync/WcSync.Sync/
COPY WcSync/WcSync.Wc/ WcSync/WcSync.Wc/
COPY Data/ Data/
WORKDIR /source/WcSync/WcSync.Cli
RUN dotnet build -c Release -o /out --no-restore

# test stage -- exposes optional entrypoint
# target entrypoint with: docker build --target test
FROM build AS test
WORKDIR /source
COPY WcSync/WcSync.Tests/*.csproj WcSync/WcSync.Tests/
RUN dotnet restore WcSync/WcSync.Tests/WcSync.Tests.csproj

COPY WcSync/WcSync.Tests/ WcSync/WcSync.Tests/
WORKDIR /source/WcSync/WcSync.Tests
RUN dotnet build --no-restore
ENTRYPOINT ["dotnet", "test", "--logger:trx", "--no-restore", "--no-build"]

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "WcSync.Cli.dll"]
