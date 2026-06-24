# How to Run FlightDex with Docker

This guide explains how to build and run the full FlightDex stack (Redis, the .NET API, and the Angular frontend) using Docker Compose.

## Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (or Docker Engine + Docker Compose v2) installed and running.
- Ports `8080`, `5162`, and `6379` free on your machine.

Verify Docker is available:

```bash
docker --version
docker compose version
```

## What gets started

`docker-compose.yml` defines three services:

| Service | Image / Build | Purpose | Published port |
| --- | --- | --- | --- |
| `redis` | `redis:7-alpine` | Caches airport suggestions | `6379` (optional, for inspection) |
| `api`   | built from `./Backend` (.NET 10) | REST API; SQLite data on a named volume | `5162 → 8080` (optional, hit API directly) |
| `web`   | built from `./Frontend` (Angular + nginx) | The UI; reverse-proxies API calls to `api` | `8080 → 80` |

The frontend is served by nginx, which reverse-proxies `/flight`, `/auth`, and `/ticket` requests to the `api` service, so everything is same-origin (no CORS setup needed).

## Run the stack

From the **repository root** (the folder containing `docker-compose.yml`):

```bash
docker compose up --build
```

- `--build` (re)builds the API and frontend images. Omit it on later runs if nothing changed.
- The first build downloads the .NET SDK and Node images, so it can take a few minutes.

To run in the background (detached):

```bash
docker compose up --build -d
```

## Access the app

Once the containers are healthy:

- **UI:** http://localhost:8080
- **API directly (optional):** http://localhost:5162/flight?at=BOM

## View logs

```bash
# all services
docker compose logs -f

# a single service
docker compose logs -f api
```

## Stop the stack

```bash
# stop and remove containers (named volumes are kept)
docker compose down

# also remove the data + redis volumes (wipes the SQLite databases)
docker compose down -v
```

## Data persistence

SQLite database files live on the `flightdex-data` named volume, mounted at `/data` in the API container:

- `Data Source=/data/flightdex.db` — flights
- `Data Source=/data/flightdex-booking.db` — bookings

Redis data is persisted on the `flightdex-redis` volume. Data survives `docker compose down` and is only removed with `docker compose down -v`.

## Configuration

The API reads configuration from environment variables set in `docker-compose.yml`:

| Variable | Default | Notes |
| --- | --- | --- |
| `ConnectionStrings__FlightDex` | `Data Source=/data/flightdex.db` | Flights database |
| `ConnectionStrings__Booking` | `Data Source=/data/flightdex-booking.db` | Bookings database |
| `ConnectionStrings__Redis` | `redis:6379` | Redis cache endpoint |
| `Jwt__Key` | dev placeholder | **Override with a long, random secret in any real deployment.** |
| `ASPNETCORE_ENVIRONMENT` | `Production` | — |

## Troubleshooting

- **Port already in use:** stop whatever is using `8080`/`5162`/`6379`, or change the host-side port mapping in `docker-compose.yml` (e.g. `"8081:80"`).
- **Stale build after code changes:** rebuild without cache with `docker compose build --no-cache` then `docker compose up`.
- **API can't reach Redis:** make sure the `redis` service is up (`docker compose ps`); the API connects to it by the service name `redis`, not `localhost`.
- **Reset everything:** `docker compose down -v` removes containers and volumes for a clean slate.
