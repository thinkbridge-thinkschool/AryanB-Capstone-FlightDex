# FlightDex — Platform Features

The frameworks, libraries, and language features actually used in FlightDex, with the
concrete APIs and file references behind each. This is a feature inventory, not a how-to —
see [how_to_run.md](how_to_run.md) to run it and [Structure.md](Structure.md) for the layout.

---

## Backend — .NET 10 / ASP.NET Core

### Architecture

- **Modular monolith + Onion/Clean Architecture.** Two bounded contexts (Flights, Booking),
  each split into `Domain` → `Application` → `Infrastructure`, over a shared `SharedKernel`.
- **Custom lightweight CQRS — not MediatR.** Hand-rolled `ICommand<T>` / `IQuery<T>`,
  `ICommandHandler` / `IQueryHandler`, and `CommandDispatcher` / `QueryDispatcher` that
  resolve handlers from DI by reflection (`FlightDex.SharedKernel/Cqrs`).
- Newer **`.slnx`** solution format (`Backend/FlightDex.slnx`).

### ASP.NET Core

- **Attribute-routed controllers** (not minimal APIs): `FlightController`, `AirportController`,
  `AuthController`, `TicketController`.
- **`public partial class Program;`** at the end of `Program.cs`, exposing the entry point so
  integration tests can boot it via `WebApplicationFactory<Program>`.
- **Per-module DI extension methods**: `AddFlightsApplication()`, `AddBookingInfrastructure()`,
  `AddCqrs()`, etc.
- **Built-in OpenAPI** via `Microsoft.AspNetCore.OpenApi` + `app.MapOpenApi()` (Development
  only) — no Swashbuckle.
- **CORS** policy `"AngularClient"`, origins read from `appsettings.json` (default
  `http://localhost:4200`).
- **Layered configuration**: `appsettings.json` + environment-variable overrides
  (e.g. `ConnectionStrings__FlightDex`) used by tests.

### Data access — EF Core 10

- **Provider: SQLite**, with two separate DbContexts / DB files — `FlightsDbContext` →
  `flightdex.db`, `BookingDbContext` → `flightdex-booking.db` — respecting the module boundary.
- `IEntityTypeConfiguration<T>` mappings applied via `ApplyConfigurationsFromAssembly()`, with
  indexes on the flight lookup columns.
- **Migrations** applied at startup with `Database.MigrateAsync()`.
- **CSV seeder** (`FlightTimetableSeeder`) — idempotent; reads bundled `Departures_*.csv` /
  `Arrivals_*.csv` copied to the output directory.
- **`IMemoryCache`** caches the (static) timetable pages; `.AsNoTracking()` on read queries.

### Auth & security

- **JWT Bearer** (`JwtTokenService`, HS256) issuing `sub` / `email` / `given_name` /
  `family_name` / `jti` claims; validated with a 30-second clock skew.
- **PBKDF2-SHA256 password hashing** (`Pbkdf2PasswordHasher`, 100k iterations, 16-byte salt)
  with constant-time comparison (`CryptographicOperations.FixedTimeEquals`).
- `[Authorize]` on ticket endpoints + ownership checks via `User.GetUserId()`.
- Explicit **NuGet security-patch overrides** (`SQLitePCLRaw.bundle_e_sqlite3 3.0.3`,
  `System.Security.Cryptography.Xml 10.0.9`).

### C# language features

File-scoped namespaces · **primary constructors** (DbContexts, repositories) · **records**
(queries, specs, `PagedResult<T>`) · nullable reference types · `sealed` throughout · pattern
matching (`is { } after`) · tuple deconstruction · `TimeOnly` / `DateOnly` · async/await + LINQ.

### Testing & CI

- **xUnit** + **NSubstitute** (mocking) + **coverlet** for unit tests of handlers, the hasher,
  and the JWT service.
- **Integration tests** with `Microsoft.AspNetCore.Mvc.Testing` (`WebApplicationFactory<Program>`,
  `IAsyncLifetime`, `[Collection]`), including a full register → book → list → cancel E2E.
- **Perf tests** tagged `[Trait("Category","Perf")]` (p50/p95/p99 latency), excluded from CI via
  `--filter "Category!=Perf"`.
- **GitHub Actions** (`.github/workflows/ci.yml`): `workflow_dispatch` trigger, .NET 10 SDK,
  restore/build/test with TRX + XPlat coverage artifacts.

---

## Frontend — Angular 22

### Setup & tooling

- **Angular 22.0.0**, **TypeScript ~6.0**, built with the modern esbuild-based
  **`@angular/build:application`** builder; dev server uses `proxy.conf.json` to reach the API.
- Strict TS config (`noImplicitOverride`, `noPropertyAccessFromIndexSignature`, etc.).

### Component model — fully standalone + signals

- **100% standalone components** (`imports: [...]`, no NgModules).
- **Signals-first reactivity**: `signal()` for mutable UI/form state, `computed()` for derived
  values (`onlyOneVisible`, `passengerName`), and **`input()` / `output()`** signal APIs on the
  reusable `Autocomplete` component.
- **`inject()` function** for all DI — no constructor-parameter injection.

### Templates

- **New control-flow syntax** throughout: `@if` / `@else` / `@for` with mandatory `track`, plus
  `@if (...; as r)` aliasing. No `*ngIf` / `*ngFor`.
- Property/event binding, class binding (`[class.single]`), and a signals-style two-way pattern:
  `[ngModel]="email()" (ngModelChange)="email.set($event)"`.
- A **custom directive** `ShowPickerDirective` calling native `input.showPicker()`.

### Routing

Defined in `src/app/app.routes.ts`: **lazy `loadComponent()`** imports for every route, per-route
`title`, a **functional `CanActivateFn` guard** (`authGuard`) protecting `/book` and `/mytickets`
with a `returnUrl` redirect, and a `**` wildcard fallback.

### Services, DI & HTTP

- Tree-shakable services (`@Injectable({ providedIn: 'root' })`): `AuthService`, `FlightService`,
  `TicketService`.
- **`provideHttpClient(withFetch(), withInterceptors([authInterceptor]))`** — Fetch backend + a
  **functional HTTP interceptor** that attaches `Authorization: Bearer <token>`.
- `HttpClient` with `HttpParams`; relative URLs proxied to the backend.

### Reactive features

- **RxJS** (`Observable`, `map`, `tap`) for HTTP streams — `tap` to persist the session on login,
  `map` to normalize responses. Components subscribe directly (no `async` pipe).
- **Template-driven forms only** (`FormsModule`) — no `ReactiveFormsModule` / `FormBuilder`;
  validation is manual in component methods.
- `AuthService` keeps the session as a `signal`, with `computed` `user` / `isAuthenticated`,
  persisted to **localStorage** with JWT-expiry checks.

### Styling & state

- Global `styles.css` with **CSS custom properties** for theming + per-component external/inline
  styles (Angular view encapsulation).
- **No state-management library** (no NgRx/Redux) — signals + services is the entire strategy.

---

## Big picture

Both sides deliberately favor lightweight, modern primitives over heavyweight frameworks — custom
CQRS instead of MediatR on the backend, and signals + standalone components instead of
NgModules/NgRx on the frontend — running on the latest .NET 10 and Angular 22 releases.
