# --- build ---
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore against the solution so all project references resolve.
COPY . .
RUN dotnet restore FlightDex.slnx

# Publish the API. No apphost exe needed; we launch via `dotnet FlightDex.Api.dll`.
RUN dotnet publish src/FlightDex.Api/FlightDex.Api.csproj -c Release -o /app /p:UseAppHost=false

# --- runtime ---
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .

# SQLite database file lives on a mounted volume (see docker-compose.yml).
RUN mkdir -p /data
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "FlightDex.Api.dll"]
