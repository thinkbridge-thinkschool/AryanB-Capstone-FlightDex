# FlightDex — Solution Structure

```
FlightDex/
├── FlightDex.sln                                  # Solution wiring all module + host + test projects
├── Directory.Build.props                          # Shared MSBuild settings for every project
├── Design.md                                      # One-page design: contexts, aggregates, async flows
├── Structure.md                                   # This file: folder and file structure
│── D22P1_Solution.md                              # Solution of Day 22 Piece 2
│
├── diagrams/
│   ├── contexts.mmd                               # Section 1 mermaid: bounded-context map
│   ├── aggregates.mmd                             # Section 2 mermaid: Flight aggregate + references
│   └── async-flows.mmd                            # Section 3 mermaid: event refresh + read path
│
├── src/
│   ├── Bootstrap/
│   │   └── FlightDex.Api/                          # Web host and composition root
│   │       ├── FlightDex.Api.csproj
│   │       ├── Program.cs                          # App entry point; wires modules into the host
│   │       ├── appsettings.json                   # Host configuration
│   │       ├── Endpoints/
│   │       │   └── FlightEndpoints.cs              # Maps /flight and /flight/{flightId} routes
│   │       └── Modules/
│   │           └── ModuleRegistration.cs          # Registers each bounded context's services
│   │
│   ├── Shared/
│   │   ├── FlightDex.SharedKernel/                 # Cross-context domain primitives
│   │   │   ├── FlightDex.SharedKernel.csproj
│   │   │   ├── Domain/
│   │   │   │   ├── AggregateRoot.cs                # Base type for aggregate roots
│   │   │   │   ├── Entity.cs                       # Base type for entities
│   │   │   │   ├── ValueObject.cs                  # Base type for value objects
│   │   │   │   └── IDomainEvent.cs                 # Marker for domain events
│   │   │   ├── Pagination/
│   │   │   │   ├── PagedRequest.cs                 # Page number/size input contract
│   │   │   │   └── PagedResult.cs                  # Generic paginated response envelope
│   │   │   └── Abstractions/
│   │   │       └── IUnitOfWork.cs                  # Transaction boundary abstraction
│   │   │
│   │   └── FlightDex.IntegrationEvents/            # Public event contracts between contexts
│   │       ├── FlightDex.IntegrationEvents.csproj
│   │       ├── IEventBus.cs                        # In-process event bus abstraction
│   │       ├── IIntegrationEvent.cs                # Marker for integration events
│   │       ├── FlightUpsertedEvent.cs              # Raised when a flight is created/updated
│   │       ├── RouteChangedEvent.cs                # Raised when a route's endpoints change
│   │       └── LocationChangedEvent.cs             # Raised when a location's details change
│   │
│   └── Modules/
│       ├── Timetable/                              # CORE context: owns Flight + FlightDetails
│       │   ├── FlightDex.Timetable.Domain/
│       │   │   ├── FlightDex.Timetable.Domain.csproj
│       │   │   ├── Flight/
│       │   │   │   ├── Flight.cs                   # Flight aggregate root
│       │   │   │   ├── FlightDetails.cs            # Detail entity within the Flight aggregate
│       │   │   │   ├── FlightId.cs                 # Strongly-typed flight identifier
│       │   │   │   ├── SeatingPlan.cs              # Value object: seats by class
│       │   │   │   ├── FlightSchedule.cs           # Value object: departure/arrival timestamps
│       │   │   │   └── IFlightRepository.cs        # Persistence port for the Flight aggregate
│       │   │   └── ReadModels/
│       │   │       └── FlightView.cs               # Denormalized dashboard/detail read model
│       │   ├── FlightDex.Timetable.Application/
│       │   │   ├── FlightDex.Timetable.Application.csproj
│       │   │   ├── Queries/
│       │   │   │   ├── GetFlightTimetable/
│       │   │   │   │   ├── GetFlightTimetableQuery.cs    # Paginated timetable query input
│       │   │   │   │   └── GetFlightTimetableHandler.cs  # Returns a page of flight summaries
│       │   │   │   └── GetFlightDetails/
│       │   │   │       ├── GetFlightDetailsQuery.cs      # Single-flight detail query input
│       │   │   │       └── GetFlightDetailsHandler.cs    # Returns enriched flight details
│       │   │   ├── Contracts/
│       │   │   │   ├── FlightSummaryDto.cs         # Timetable row shape
│       │   │   │   └── FlightDetailsDto.cs         # Expanded detail panel shape
│       │   │   ├── Enrichment/
│       │   │   │   ├── IRouteLookup.cs             # Port to resolve RouteID -> airport codes
│       │   │   │   ├── ILocationLookup.cs          # Port to resolve airport code -> city/state/country
│       │   │   │   └── FlightViewProjector.cs      # Builds FlightView from flight + route + location
│       │   │   └── EventHandlers/
│       │   │       ├── RouteChangedHandler.cs      # Refreshes FlightViews on route change
│       │   │       └── LocationChangedHandler.cs   # Refreshes FlightViews on location change
│       │   └── FlightDex.Timetable.Infrastructure/
│       │       ├── FlightDex.Timetable.Infrastructure.csproj
│       │       ├── Persistence/
│       │       │   ├── TimetableDbContext.cs       # EF Core context for Flight/FlightDetails/FlightView
│       │       │   ├── FlightRepository.cs         # IFlightRepository implementation
│       │       │   └── Configurations/
│       │       │       ├── FlightConfiguration.cs        # Flight table mapping
│       │       │       ├── FlightDetailsConfiguration.cs # FlightDetails table mapping
│       │       │       └── FlightViewConfiguration.cs    # FlightView read-model mapping
│       │       ├── Lookups/
│       │       │   ├── RouteLookupAdapter.cs       # IRouteLookup over the Routing context
│       │       │   └── LocationLookupAdapter.cs    # ILocationLookup over the Locations context
│       │       └── TimetableModule.cs              # DI registration for the Timetable context
│       │
│       ├── Routing/                                # Owns Routes (RouteID -> from/to airport codes)
│       │   ├── FlightDex.Routing.Domain/
│       │   │   ├── FlightDex.Routing.Domain.csproj
│       │   │   ├── Route.cs                        # Route aggregate root
│       │   │   ├── RouteId.cs                      # Strongly-typed route identifier
│       │   │   ├── AirportCode.cs                  # Value object for airport code
│       │   │   └── IRouteRepository.cs             # Persistence port for routes
│       │   ├── FlightDex.Routing.Application/
│       │   │   ├── FlightDex.Routing.Application.csproj
│       │   │   ├── Queries/
│       │   │   │   └── GetRouteById/
│       │   │   │       ├── GetRouteByIdQuery.cs    # Route lookup input
│       │   │   │       └── GetRouteByIdHandler.cs  # Returns route endpoints
│       │   │   └── Contracts/
│       │   │       └── RouteDto.cs                 # Route endpoint shape exposed to other contexts
│       │   └── FlightDex.Routing.Infrastructure/
│       │       ├── FlightDex.Routing.Infrastructure.csproj
│       │       ├── Persistence/
│       │       │   ├── RoutingDbContext.cs         # EF Core context for Routes
│       │       │   ├── RouteRepository.cs          # IRouteRepository implementation
│       │       │   └── Configurations/
│       │       │       └── RouteConfiguration.cs   # Routes table mapping
│       │       └── RoutingModule.cs                # DI registration for the Routing context
│       │
│       └── Locations/                              # Owns Locations (airport code -> city/state/country)
│           ├── FlightDex.Locations.Domain/
│           │   ├── FlightDex.Locations.Domain.csproj
│           │   ├── Location.cs                     # Location aggregate root
│           │   ├── AirportCode.cs                  # Strongly-typed airport code identifier
│           │   ├── Address.cs                      # Value object: city/state/country
│           │   └── ILocationRepository.cs          # Persistence port for locations
│           ├── FlightDex.Locations.Application/
│           │   ├── FlightDex.Locations.Application.csproj
│           │   ├── Queries/
│           │   │   └── GetLocationByCode/
│           │   │       ├── GetLocationByCodeQuery.cs    # Location lookup input
│           │   │       └── GetLocationByCodeHandler.cs  # Returns location details
│           │   └── Contracts/
│           │       └── LocationDto.cs              # Location detail shape exposed to other contexts
│           └── FlightDex.Locations.Infrastructure/
│               ├── FlightDex.Locations.Infrastructure.csproj
│               ├── Persistence/
│               │   ├── LocationsDbContext.cs       # EF Core context for Locations
│               │   ├── LocationRepository.cs       # ILocationRepository implementation
│               │   └── Configurations/
│               │       └── LocationConfiguration.cs # Locations table mapping
│               └── LocationsModule.cs              # DI registration for the Locations context
│
└── tests/
    ├── FlightDex.Timetable.Tests/                  # Tests for the Timetable context
    │   └── FlightDex.Timetable.Tests.csproj
    ├── FlightDex.Routing.Tests/                    # Tests for the Routing context
    │   └── FlightDex.Routing.Tests.csproj
    └── FlightDex.Locations.Tests/                  # Tests for the Locations context
        └── FlightDex.Locations.Tests.csproj
```
