FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /source

# copy and build app and libraries
COPY WcSync/WcSync.Cli/ WcSync/WcSync.Cli/
COPY WcSync/WcSync.Model/ WcSync/WcSync.Model/
COPY WcSync/WcSync.Sync/ WcSync/WcSync.Sync/
COPY WcSync/WcSync.Wc/ WcSync/WcSync.Wc/
COPY Data/ Data/
RUN dotnet build WcSync/WcSync.Cli/WcSync.Cli.csproj -c Release -o /out

# test stage -- exposes optional entrypoint
# target entrypoint with: docker build --target test
FROM build AS test
COPY WcSync/WcSync.Tests/ WcSync/WcSync.Tests/
RUN dotnet test WcSync/WcSync.Tests/WcSync.Tests.csproj

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:7.0
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "WcSync.Cli.dll"]
