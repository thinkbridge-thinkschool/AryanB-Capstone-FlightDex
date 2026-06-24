# FlightDex — APIs & Query Flow

How every UI field and operation flows to the backend, what loads by default, and which
queries each action issues. Read top-to-bottom or jump to a screen.

**Request path:** `UI field/operation` → `Angular service` → `HTTP API` → `Controller`
→ `CQRS query/command` → `Repository` → `Azure/SQLite DB` **or** `Redis` **or** `in-code list`.

Every request to `/flight`, `/airports`, `/auth`, `/ticket` is reverse-proxied to the API
(nginx in Docker, `proxy.conf.json` in `ng serve`). The auth interceptor attaches the bearer
token to outgoing calls; `/ticket/*` requires it.

---

## 1. Routes & what loads by default

| Route | Auth | On open it fires | Resulting calls |
|---|---|---|---|
| `/` | – | redirect → `/timetable` | – |
| `/timetable` | public | `loadDepartures()`, `loadArrivals()`, `getAirportSuggestions()` | `GET /flight?at=PNQ&to=` · `GET /flight?at=PNQ&from=` · `GET /airports/suggestions` |
| `/login` | public | nothing | – |
| `/book` | **guard** | `getAirportSuggestions()` | `GET /airports/suggestions` |
| `/mytickets` | **guard** | `load()` | `GET /ticket` |
| `**` | – | redirect → `/timetable` | – |

**Timetable default:** the "currently at" airport defaults to **PNQ**, and the page opens
showing **both** PNQ departures and arrivals (page 1, 30 rows each, sorted by time).

---

## 2. Timetable screen

Default airport `at = PNQ`. Two result boxes (departures / arrivals), each paged at 30.

### Fields → suggestions source

| Field | Suggestions from | Hits API? |
|---|---|---|
| **You are currently at** (`airportSearch`) | In-code `ALL_AIRPORT_ALIASES` (5 served airports: code/name/city) | No |
| **Arriving From** (`arriveFrom`) | Redis cache (`getAirportSuggestions`, loaded once) | `GET /airports/suggestions` |
| **Departing To** (`departTo`) | Redis cache (loaded once) | `GET /airports/suggestions` |

### Operations → queries

| Operation | Method | Query sent | Notes |
|---|---|---|---|
| Pick/enter an airport (`onAirportSelected`) | `resolveAirport` (local) → `at.set()` → `showBoth()` | `GET /flight?at={CODE}&to=` and `GET /flight?at={CODE}&from=` | `at` must resolve to BLR/BOM/PNQ/LON/DBX; resets to both-boxes view, page 1 |
| **Search** (`doSearch`) | `loadDepartures` and/or `loadArrivals` | `GET /flight?at=&to={departTo}&deptTime_After=&deptTime_Before=` and/or `GET /flight?at=&from={arriveFrom}&arrTime_After=&arrTime_Before=` | View narrows to departures-only / arrivals-only / both based on which fields are filled |
| Time filters (`deptAfter/deptBefore/arrAfter/arrBefore`) | sent with Search | added as `deptTime_*` / `arrTime_*` (`HHMM`) | No call on their own |
| Prev/Next page | `goToDeparturePage` / `goToArrivalPage` | `GET /flight?...&page={n}&pageSize=30` | Per box |
| Click a flight row (`openDetail`) | `getDetail` | `GET /flight/{flightCode}` | Opens the detail modal |
| "Show departures & arrivals" (`showBoth`) | clears filters, reloads | both `GET /flight` calls | – |

---

## 3. Book Tickets screen (auth required)

### Fields → suggestions source

| Field | Suggestions from | Hits API? |
|---|---|---|
| **From** (`from`) | In-code `ALL_AIRPORT_ALIASES` (served airports only) | No |
| **To** (`to`) | Redis cache (`getAirportSuggestions`, loaded once) | `GET /airports/suggestions` |
| **Date** (`date`) | – (native date picker; text↔date swap for placeholder) | No |

### Operations → queries

| Operation | Method | Query/command sent | Notes |
|---|---|---|---|
| **Search Flights** (`search`) | `resolveAirport(from)` (local) → `getDepartures` | `GET /flight?at={ORIGIN}&to={to}&page=1&pageSize=100` | Origin must be a served airport, else a client-side error (no call) |
| Select a flight (`selectFlight`) | — | none | Opens the confirm modal |
| **Confirm & Book** (`confirmBooking`) | `tickets.book` | `POST /ticket` `{date,time,origin{code,airport,city},destination{code,airport,city}}` | Returns the created `Ticket`; clears the result list |
| Back/dismiss | — | none | – |

---

## 4. Login / Register screen

| Operation | Fields | Command sent |
|---|---|---|
| **Login** (`submitLogin`) | email, password | `POST /auth/login` `{email,password}` → token persisted to localStorage |
| Register **step 1 → Next** | email, firstName, lastName, age, isGovernmentOfficial, isLawEnforcementOrMilitary | none (client-side step) |
| Register **step 2 → Register** (`submitRegister`) | password, confirmPassword | `POST /auth/register` `{...all fields...}` → token persisted |
| Logout | — | none (clears localStorage) |

On success both calls store `{token, user, expiresAtUtc}`; a refresh restores the session
until the token expires.

---

## 5. My Tickets screen (auth required)

| Operation | Method | Query/command sent |
|---|---|---|
| **Load** (on open, and after cancel) | `tickets.getMine` | `GET /ticket` → user's tickets, newest first |
| Select a ticket | — | none (client-side) |
| **Cancel → Confirm** (`confirmCancel`) | `tickets.cancel` | `DELETE /ticket/{id}` → then reloads `GET /ticket` |

---

## 6. API endpoint reference

| Method & path | Auth | Dispatches | Repository / store | DB/cache op |
|---|---|---|---|---|
| `GET /flight?at&to&from&deptTime_*&arrTime_*&page&pageSize` | public | `GetFlightsQuery` | `FlightRepository.GetPagedAsync` | `COUNT(*)` + paged `SELECT` over `Flights` (filtered by direction, airport, counterpart code/city/name, time) |
| `GET /flight/{flightCode}` | public | `GetFlightByCodeQuery` | `FlightRepository.GetByFlightCodeAsync` | `SELECT ... WHERE FlightCode=@code` |
| `GET /airports/suggestions` | public | — | `RedisAirportSuggestionCache.GetAllAsync` | Redis `SMEMBERS` (no DB) |
| `POST /auth/register` | public | `RegisterUserCommand` | `UserRepository.EmailExistsAsync` + `AddAsync` | `EXISTS` check, then `INSERT` user |
| `POST /auth/login` | public | `LoginCommand` | `UserRepository.GetByEmailAsync` | `SELECT user WHERE Email=@email` |
| `POST /ticket` | bearer | `BookTicketCommand` | `TicketRepository.AddAsync` | `INSERT` ticket for `User.GetUserId()` |
| `GET /ticket` | bearer | `GetMyTicketsQuery` | `TicketRepository.GetByUserAsync` | `SELECT ... WHERE UserId=@id ORDER BY Id DESC` |
| `DELETE /ticket/{id}` | bearer | `CancelTicketCommand` | `TicketRepository.GetByIdAsync` + `RemoveAsync` | `SELECT` then `DELETE` (only if owned) |

---

## 7. Backend queries (the read side)

**CQRS queries**
- `GetFlightsQuery(spec)` → paged `FlightListItem` (timetable & booking search).
- `GetFlightByCodeQuery(code)` → `FlightDetail` (detail modal).
- `GetMyTicketsQuery(userId)` → `TicketDto[]` (My Tickets).

**EF Core reads**
- `FlightRepository.GetPagedAsync` — `WHERE Direction=@d [AND Airport=@at] [AND (CounterpartCode=@t OR CounterpartCity=@t OR CounterpartAirport=@t)] [AND ScheduledTime BETWEEN ...]`, then `COUNT(*)` + `ORDER BY ScheduledTime, FlightCode LIMIT/OFFSET`.
- `FlightRepository.GetByFlightCodeAsync` — `WHERE FlightCode=@code ORDER BY Direction, ScheduledTime`.
- `UserRepository` — `GetByEmailAsync`, `GetByIdAsync`, `EmailExistsAsync`.
- `TicketRepository` — `GetByUserAsync`, `GetByIdAsync`.

**Startup cache extract** (`AirportSuggestionCacheBuilder.RebuildAsync`, after seeding)
- `SELECT DISTINCT` over `CounterpartCode`, `CounterpartAirport`, `CounterpartCity`, `Airport`
  → unioned/deduped (case-insensitive) → written to Redis set `flightdex:airport-suggestions`.

**Redis**
- Read: `SMEMBERS flightdex:airport-suggestions` (powers `/airports/suggestions`).
- Rebuild: `DEL` + `SADD`.

---

## 8. Data-source summary

| Concern | Source |
|---|---|
| Served-airport suggestions (timetable "currently at", booking "From") | **In-code** (`AIRPORT_INFO` → `ALL_AIRPORT_ALIASES`) — no DB, no cache |
| Counterpart suggestions (timetable "Arriving From"/"Departing To", booking "To") | **Redis cache** (`/airports/suggestions`) |
| Actual flight search / detail | **Database** (`Flights` table) |
| Auth & tickets | **Database** (`Users`, `Tickets`) |
