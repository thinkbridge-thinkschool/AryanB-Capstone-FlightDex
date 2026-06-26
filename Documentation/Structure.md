# FlightDex — Solution Structure

A modular monolith: one ASP.NET Core host composing two vertical-slice modules (Flights,
Booking) over a shared CQRS kernel, plus an Angular front end. Each module keeps its own
Domain / Application / Infrastructure projects and its own SQLite database.

```
FlightDex/
├── ReadMe.md                                         # Project overview
├── docker-compose.yml                                # api + web services; SQLite on a named volume
│
├── Documentation/
│   ├── Design.md                                     # One-page design: modules, aggregates, flows
│   ├── Structure.md                                  # This file: folder and file structure
│   ├── Flow.md                                       # Every UI field/operation → API → query/command
│   ├── BuildPlan.md                                  # Build roadmap
│   ├── how_to_run.md                                 # Local + Docker run instructions
│   ├── ADRs/                                          # Architecture decision records
│   ├── Solutions/                                     # Per-day exercise solutions
│   └── Snapshots/                                     # UI + CI screenshots
│
├── Backend/
│   ├── FlightDex.slnx                                # Solution wiring host + modules + tests
│   ├── Dockerfile                                    # Builds + publishes the API image
│   ├── src/
│   │   ├── FlightDex.Api/                             # Web host and composition root
│   │   │   ├── Program.cs                             # Wires CQRS + both modules; migrate→seed→extract; JWT/CORS
│   │   │   ├── appsettings.json                       # Connection strings (FlightDex, Booking), Jwt, Cors
│   │   │   ├── ClaimsPrincipalExtensions.cs           # User.GetUserId() from the bearer token
│   │   │   ├── Contracts/
│   │   │   │   ├── AuthContracts.cs                   # RegisterRequest / LoginRequest bodies
│   │   │   │   └── TicketContracts.cs                 # BookTicketRequest body
│   │   │   └── Controllers/
│   │   │       ├── FlightController.cs                # GET /flight , GET /flight/{flightCode}
│   │   │       ├── AirportController.cs               # GET /airports/suggestions
│   │   │       ├── AuthController.cs                  # POST /auth/register , /auth/login
│   │   │       └── TicketController.cs                # POST/GET/DELETE /ticket  ([Authorize])
│   │   │
│   │   ├── FlightDex.SharedKernel/                    # Cross-module primitives (no domain logic)
│   │   │   ├── Cqrs/
│   │   │   │   ├── ICommand.cs / IQuery.cs            # Marker contracts for commands / queries
│   │   │   │   ├── ICommandHandler.cs / IQueryHandler.cs
│   │   │   │   ├── ICommandDispatcher.cs / IQueryDispatcher.cs
│   │   │   │   ├── CommandDispatcher.cs / QueryDispatcher.cs   # Resolve + invoke the handler
│   │   │   │   └── SharedKernelServiceCollectionExtensions.cs # AddCqrs()
│   │   │   └── Paging/
│   │   │       └── PagedResult.cs                     # Generic page envelope (+ Total/HasNext/...)
│   │   │
│   │   ├── FlightDex.Flights.Domain/                  # Flights: domain
│   │   │   ├── Flight.cs                              # Unified timetable row (departures + arrivals)
│   │   │   ├── FlightDirection.cs                     # Departure | Arrival
│   │   │   └── Location.cs                            # Derived airport-suggestion row
│   │   ├── FlightDex.Flights.Application/             # Flights: use cases (read-only)
│   │   │   ├── DependencyInjection.cs                 # AddFlightsApplication()
│   │   │   ├── Flights.cs                             # ServedAirports (BLR/BOM/PNQ/LON/DBX)
│   │   │   ├── Abstractions/
│   │   │   │   ├── IFlightRepository.cs               # Timetable read port
│   │   │   │   ├── FlightQuerySpec.cs                 # Direction/airport/term/code/time/page filter set
│   │   │   │   ├── IAirportSuggestionCache.cs         # Suggestion read port
│   │   │   │   └── AirportSuggestion.cs               # (Code, Name, City)
│   │   │   ├── Dtos/
│   │   │   │   ├── FlightListItem.cs                  # Timetable row shape (FromDomain)
│   │   │   │   └── FlightDetail.cs                    # Detail-modal shape (FromDomain)
│   │   │   └── Queries/
│   │   │       ├── GetFlights/                        # Query + handler → PagedResult<FlightListItem>
│   │   │       └── GetFlightByCode/                   # Query + handler → FlightDetail[]
│   │   └── FlightDex.Flights.Infrastructure/          # Flights: persistence + adapters
│   │       ├── DependencyInjection.cs                 # AddFlightsInfrastructure() (SQLite + MemoryCache)
│   │       ├── Persistence/
│   │       │   ├── FlightsDbContext.cs                # EF Core context: Flights, Locations
│   │       │   ├── FlightsDbContextFactory.cs         # Design-time factory for migrations
│   │       │   ├── FlightRepository.cs                # IFlightRepository (IMemoryCache → SQLite)
│   │       │   ├── Configurations/                    # FlightConfiguration, LocationConfiguration
│   │       │   ├── Migrations/                        # InitialCreate, AddLocations, RestructureLocations
│   │       │   ├── Seeding/
│   │       │   │   ├── FlightTimetableSeeder.cs       # Loads Departures_*/Arrivals_* CSVs at startup
│   │       │   │   └── CsvLineParser.cs               # Positional CSV line parser
│   │       │   └── SeedData/                          # 10 CSVs: {Departures,Arrivals}_{BLR,BOM,PNQ,LON,DBX}
│   │       └── Caching/
│   │           ├── AirportSuggestionCacheBuilder.cs   # Extracts Locations from the timetable (startup)
│   │           └── SqliteAirportSuggestionCache.cs    # IAirportSuggestionCache over the Locations table
│   │
│   │       ── Booking module (parallels the Flights layout) ──
│   │   ├── FlightDex.Booking.Domain/
│   │   │   ├── User.cs                                # Account; PBKDF2 hash + salt
│   │   │   └── Ticket.cs                              # Owned by User; snapshots airports + passenger
│   │   ├── FlightDex.Booking.Application/
│   │   │   ├── DependencyInjection.cs                 # AddBookingApplication()
│   │   │   ├── BookingExceptions.cs                   # EmailAlreadyInUse / InvalidCredentials
│   │   │   ├── Abstractions/                          # IUserRepository, ITicketRepository,
│   │   │   │                                          #   IPasswordHasher, IJwtTokenService
│   │   │   ├── Dtos/                                  # AuthResult, UserDto, TicketDto
│   │   │   ├── Commands/                              # RegisterUser, Login, BookTicket, CancelTicket
│   │   │   └── Queries/
│   │   │       └── GetMyTickets/                      # Query + handler → TicketDto[]
│   │   └── FlightDex.Booking.Infrastructure/
│   │       ├── DependencyInjection.cs                 # AddBookingInfrastructure() (SQLite + JWT + hasher)
│   │       ├── Persistence/
│   │       │   ├── BookingDbContext.cs                # EF Core context: Users, Tickets
│   │       │   ├── BookingDbContextFactory.cs         # Design-time factory
│   │       │   ├── UserRepository.cs / TicketRepository.cs
│   │       │   ├── Configurations/                    # UserConfiguration, TicketConfiguration
│   │       │   └── Migrations/                        # InitialCreate
│   │       └── Security/
│   │           ├── JwtOptions.cs / JwtTokenService.cs # Issues + signs bearer tokens
│   │           └── Pbkdf2PasswordHasher.cs            # IPasswordHasher (PBKDF2)
│   │
│   └── tests/
│       ├── FlightDex.UnitTests/                       # Handlers, DTO mapping, hashing, JWT, paging
│       │   ├── Flights/                               # GetFlights/GetFlightByCode handlers, mapping, ServedAirports
│       │   ├── Booking/                               # Register/Login/Book/Cancel handlers, PBKDF2, JWT
│       │   └── SharedKernel/                          # PagedResult
│       └── FlightDex.IntegrationTests/                # WebApplicationFactory<Program> end-to-end
│           ├── Infrastructure/                        # FlightDexApiFactory, test contracts
│           ├── FlightApiTests.cs / FlightRepositoryTests.cs
│           ├── AuthApiTests.cs / BookingJourneyE2ETests.cs
│           └── PerfTests.cs                           # Query latency checks
│
└── Frontend/                                          # Angular standalone app (signals)
    ├── package.json / angular.json / tsconfig*.json
    ├── proxy.conf.json                                # ng serve → API (/flight,/airports,/auth,/ticket)
    ├── nginx.conf / Dockerfile                         # Serves the build + reverse-proxies the API
    ├── public/                                         # favicon, TitleBackground.jpg
    └── src/
        ├── main.ts / index.html
        └── app/
            ├── app.routes.ts                          # /timetable (public), /login, /book*, /mytickets* (*=guard)
            ├── app.config.ts / app.ts / app.html
            ├── flight.models.ts                       # Airport types, AIRPORT_INFO, SERVED_AIRPORT_OPTIONS, resolveAirport
            ├── flight.service.ts                      # /flight + /airports calls
            ├── timetable/                              # Departures/arrivals boards + detail modal
            ├── book/                                   # book-tickets: search + staged booking flow
            ├── tickets/                                # my-tickets (list/cancel) + ticket.service/models
            ├── auth/                                   # login/register, auth.service, guard, interceptor, models
            └── shared/                                 # autocomplete, show-picker directive, http-errors
```
