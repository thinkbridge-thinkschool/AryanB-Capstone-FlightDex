# Build + run the FlightDex API as a container, so the ZAP pen test needs nothing
# installed on the host (no host-side dll execution for Defender to block).
# Build context is the repo root: docker build -f Backend/src/Bootstrap/FlightDex.Api/Dockerfile .

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish Backend/src/Bootstrap/FlightDex.Api/FlightDex.Api.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
# curl is only here to back the container HEALTHCHECK used by docker compose.
RUN apt-get update \
 && apt-get install -y --no-install-recommends curl \
 && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "FlightDex.Api.dll"]
