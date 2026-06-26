# FlightDex — APIs & Query Flow

How every UI field and operation flows to the backend, what loads by default, and which
queries each action issues. Read top-to-bottom or jump to a screen.

**Request path:** `UI field/operation` → `Angular service` → `HTTP API` → `Controller`
→ `CQRS query/command` → `Repository` → `SQLite DB` (timetable, users, tickets) **or**
`in-code list` (the 5 served airports).

Every request to `/flight`, `/airports`, `/auth`, `/ticket` is reverse-proxied to the API
(nginx in Docker, `proxy.conf.json` in `ng serve`). The auth interceptor attaches the bearer
token to outgoing calls; only `/ticket/*` requires it.

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

**Timetable default:** the "currently at" airport is restored from `localStorage`
(`flightdex.airport`), falling back to **PNQ**. The page opens showing **both** departures
and arrivals for that airport (page 1, 30 rows each, sorted by time).

---

## 2. Timetable screen

Default airport `at` (persisted; PNQ fallback). Two result boxes (departures / arrivals),
each paged at 30, each with its own search dropdown. Sliders toggle which boxes show and
24-hour vs AM/PM time display.

### Fields → suggestions source

| Field | Suggestions from | Hits API? |
|---|---|---|
| **Change Airport** (`airportSearch`) | In-code `SERVED_AIRPORT_OPTIONS` (5 served airports, via `resolveAirport`) | No |
| **Arriving From** (`arriveFrom`) | `/airports/suggestions` (Locations table, loaded once) | `GET /airports/suggestions` |
| **Departing To** (`departTo`) | `/airports/suggestions` (loaded once) | `GET /airports/suggestions` |
| **Search by flight code** (`departByCode` / `arriveByCode`) | – (plain text field) | No |

### Operations → queries

| Operation | Method | Query sent | Notes |
|---|---|---|---|
| Change the airport (`applyAirportSearch`) | `resolveAirport` (local) → `at.set()` → reload both | `GET /flight?at={CODE}&to=` and `GET /flight?at={CODE}&from=` | `at` must resolve to BLR/BOM/PNQ/LON/DBX; persisted to localStorage; clears filters, both boxes, page 1 |
| **Search departures** (`searchDepartures`) | `loadDepartures` | `GET /flight?at=&to={departToTerm}&flightCode=&deptTime_After=&deptTime_Before=&page=1&pageSize=30` | Focuses the departures box (centred); resets to page 1 |
| **Search arrivals** (`searchArrivals`) | `loadArrivals` | `GET /flight?at=&from={arriveFromTerm}&flightCode=&arrTime_After=&arrTime_Before=&page=1&pageSize=30` | Focuses the arrivals box; resets to page 1 |
| Time filters (`deptAfter/deptBefore/arrAfter/arrBefore`) | sent with the box's Search | added as `deptTime_*` / `arrTime_*` (`HHMM`) | No call on their own |
| Prev/Next page | `goToDeparturePage` / `goToArrivalPage` | `GET /flight?...&page={n}&pageSize=30` | Per box; clamped to `totalPages` |
| Click a flight row (`openDetail`) | `getDetail` | `GET /flight/{flightCode}` | Opens the detail modal |
| Clear a box's search (`clearDepSearch`/`clearArrSearch`) | clears fields, re-runs that box's search | one `GET /flight` call | – |

A picked suggestion sets the term to the airport **code**; free text sends the typed text
as-is (the server matches it against code, city or airport name).

---

## 3. Book Tickets screen (auth required)

### Fields → suggestions source

| Field | Suggestions from | Hits API? |
|---|---|---|
| **From** (`from`) | In-code `SERVED_AIRPORT_OPTIONS` (served airports only, via `resolveAirport`) | No |
| **To** (`to`) | `/airports/suggestions` (Locations table, loaded once) | `GET /airports/suggestions` |
| **Date** (`date`) | – (native date picker; `min` = today) | No |
| **Time window** (`after`/`before`) | – (optional HH:mm dropdown) | No |

### Operations → queries

| Operation | Method | Query/command sent | Notes |
|---|---|---|---|
| **Search Flights** (`search`) | `resolveAirport(fromTerm)` (local) → `getDepartures` | `GET /flight?at={ORIGIN}&to={toTerm}&page=1&pageSize=100` | Origin must be a served airport and a date is required, else a client-side error (no call) |
| Click a result for details (`openDetail`) | `getDetail` | `GET /flight/{flightCode}` | Opens the details pop-up |
| Book a flight (`bookFlight`) | — | none | Opens the confirm-details box (`bookStage='confirm'`) |
| **Proceed** (`proceedBooking` → `createTicket`) | `tickets.book` | `POST /ticket` `{date,time,origin{code,airport,city},destination{code,airport,city}}` | Staged fake interface (redirect → confirming → confirmed); the real ticket is created behind it, and the UI reaches "Confirmed" even if the call fails |
| Cancel / dismiss (`cancelBook`/`closeBook`) | — | none | – |

The passenger name/age on the ticket come from the **signed-in user's profile** (set by the
API), not from the form.

---

## 4. Login / Register screen

| Operation | Fields | Command sent |
|---|---|---|
| **Login** (`submitLogin`) | email, password | `POST /auth/login` `{email,password}` → token persisted to localStorage |
| Register **step 1 → Next** (`registerNext`) | email, firstName, lastName, age, isGovernmentOfficial, isLawEnforcementOrMilitary | none (client-side validation only) |
| Register **step 2 → Register** (`submitRegister`) | password, confirmPassword | `POST /auth/register` `{...all fields...}` → token persisted |
| Logout (`auth.logout`) | — | none (clears localStorage) |

Step 2 enforces a strong password client-side (≥8 chars, lower + upper + digit, must match
confirm). On success both calls store `{token, user, expiresAtUtc}`; a refresh restores the
session until the token expires, then redirects to `returnUrl` (default `/book`).

---

## 5. My Tickets screen (auth required)

| Operation | Method | Query/command sent |
|---|---|---|
| **Load** (on open, and after a cancel) | `tickets.getMine` | `GET /ticket` → user's tickets, newest first |
| Select a ticket (`select`) | — | none (client-side) |
| **Cancel → Confirm** (`confirmCancel`) | `tickets.cancel` | `DELETE /ticket/{ticketId}` → staged fake interface; `closeCancel` then reloads `GET /ticket` |

The cancel runs a staged "cancelling → confirming → cancelled" interface and always reaches
"Cancelled" in the UI, even if the `DELETE` fails.

---

## 6. API endpoint reference

| Method & path | Auth | Dispatches | Repository / store | DB op |
|---|---|---|---|---|
| `GET /flight?at&to&from&flightCode&deptTime_*&arrTime_*&page&pageSize` | public | `GetFlightsQuery` | `FlightRepository.GetPagedAsync` | `IMemoryCache` hit, else `COUNT(*)` + paged `SELECT` over `Flights` (direction, airport, counterpart code/city/name, flight code, time) |
| `GET /flight/{flightCode}` | public | `GetFlightByCodeQuery` | `FlightRepository.GetByFlightCodeAsync` | `SELECT ... WHERE FlightCode=@code ORDER BY Direction, ScheduledTime` |
| `GET /airports/suggestions` | public | — | `SqliteAirportSuggestionCache.GetAllAsync` | `SELECT Code,Name,City FROM Locations ORDER BY Name` |
| `POST /auth/register` | public | `RegisterUserCommand` | `UserRepository.EmailExistsAsync` + `AddAsync` | `EXISTS` check, then `INSERT` user (409 on duplicate) |
| `POST /auth/login` | public | `LoginCommand` | `UserRepository.GetByEmailAsync` | `SELECT user WHERE Email=@email` (401 on bad credentials) |
| `POST /ticket` | bearer | `BookTicketCommand` | `TicketRepository.AddAsync` | `INSERT` ticket for `User.GetUserId()` |
| `GET /ticket` | bearer | `GetMyTicketsQuery` | `TicketRepository.GetByUserAsync` | `SELECT ... WHERE UserId=@id ORDER BY Id DESC` |
| `DELETE /ticket/{id}` | bearer | `CancelTicketCommand` | `TicketRepository.GetByIdAsync` + `RemoveAsync` | `SELECT` then `DELETE` (only if owned by the caller; else 404) |

---

## 7. Backend queries (the read side)

**CQRS queries**
- `GetFlightsQuery(spec)` → `PagedResult<FlightListItem>` (timetable & booking search).
- `GetFlightByCodeQuery(code)` → `FlightDetail[]` (detail modal; the controller unwraps a single match).
- `GetMyTicketsQuery(userId)` → `TicketDto[]` (My Tickets).

**EF Core reads**
- `FlightRepository.GetPagedAsync` — caches each `(filter + page)` in `IMemoryCache` (the
  timetable is static for the process lifetime); on a miss runs
  `WHERE Direction=@d [AND Airport=@at] [AND (CounterpartCode=@t OR CounterpartCity=@t OR CounterpartAirport=@t)] [AND FlightCode=@code] [AND ScheduledTime BETWEEN ...]`,
  then `COUNT(*)` + `ORDER BY ScheduledTime, FlightCode LIMIT/OFFSET`.
- `FlightRepository.GetByFlightCodeAsync` — `WHERE FlightCode=@code ORDER BY Direction, ScheduledTime`.
- `UserRepository` — `GetByEmailAsync`, `GetByIdAsync`, `EmailExistsAsync`.
- `TicketRepository` — `GetByUserAsync`, `GetByIdAsync`.

**Startup seed + extract** (`Program.cs`, after migrations)
- `FlightTimetableSeeder.SeedAsync` — loads the bundled `Departures_*`/`Arrivals_*` CSVs into
  `Flights` (no-op if already seeded).
- `AirportSuggestionCacheBuilder.RebuildAsync` — `SELECT DISTINCT` over
  `(CounterpartCode, CounterpartAirport, CounterpartCity)`, collapses to one row per code, and
  writes them to the `Locations` table (no-op if already built).

---

## 8. Data-source summary

| Concern | Source |
|---|---|
| Served-airport options (timetable "Change Airport", booking "From") | **In-code** (`AIRPORT_INFO` → `SERVED_AIRPORT_OPTIONS`, `resolveAirport`) — no DB |
| Counterpart suggestions (timetable "Arriving From"/"Departing To", booking "To") | **`Locations` table** via `GET /airports/suggestions` |
| Actual flight search / detail | **Database** (`Flights` table, `IMemoryCache` in front) |
| Auth & tickets | **Database** (`Users`, `Tickets` in the separate Booking DB) |
