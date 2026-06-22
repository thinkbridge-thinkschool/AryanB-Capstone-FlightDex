# FlightDex — 4-Day Build Plan

## Slices

1. **Timetable** — API + Angular page to browse flights by departure location/time and arrival location/time. Entries are seeded by an **airline-admin** user.
2. **Ticket Booking** — API + Angular flow where a **user** clicks a timetable entry to book. Booking `POST`s the data and decrements `SeatsAvailable` by 1.
3. **User Profile** — API + Angular page where a **user** tracks their booked tickets and the flights they belong to.

---

## Day 1 — 2026-06-23 — Foundation + Feature Implementation

**Goal:** all three slices working end-to-end against seeded data (happy path).

1. **Foundation.**
   1. Verify backend builds and runs: `dotnet build FlightDex.sln`, API + worker up via `docker-compose`.
   2. Scaffold Angular under `Frontend/`: routing, `HttpClient`, environment config (API base URL), shared layout/nav.
   3. Extend `DatabaseSeeder` with seeded users — one `airline-admin`, one `user` — and add `SeatsAvailable` to the `Flight` aggregate + `FlightView` (seed each flight with a starting count).
2. **Timetable slice.**
   1. Backend: confirm `GET /v1/flight` filtering (`from`/`to`) and sort (departure/arrival/airline) behave; include `SeatsAvailable` in the response payload.
   2. Angular: timetable page that lists flights with departure location/time and arrival location/time, plus from/to filter inputs.
3. **Booking slice.**
   1. Backend: new `Booking` module + `POST /v1/bookings` (`{ flightId, userId }`) that persists a booking and decrements `SeatsAvailable` by 1 in one transaction. Decide cross-module mechanism (ID-based call vs. integration event); per [ADR 0002](adr/0002-feature-ticket-booking.md) keep it plain CRUD.
   2. Angular: a "Book" button on each timetable row that POSTs and refreshes the row's seat count.
4. **User Profile slice.**
   1. Backend: `GET /v1/users/{id}/bookings` returning the user's tickets joined to `FlightView` (flight, route, times, status).
   2. Angular: profile page listing the user's tickets with each flight's status.

**End-of-day acceptance:** admin-seeded flights show in the timetable → a user books a seat and the count drops by 1 → the booking appears on the user's profile.

## Day 2 — 2026-06-24 — Feature Completeness

**Goal:** close the gaps so each slice is fully usable, not just the happy path.

1. **Timetable.**
   1. Pagination + sort edge cases (empty pages, invalid sort key falls back to departure).
   2. Airline-admin create/update of a flight, reflected to `FlightView` via the existing `FlightUpsertedEvent` reprojection.
   3. Angular: paging/filter controls and an admin-only entry form.
2. **Booking.**
   1. Reject when `SeatsAvailable == 0` (sold out) and prevent the same user double-booking the same flight.
   2. Return a clean, typed error contract (status code + message).
   3. Angular: surface those errors as user-facing messages and disable "Book" when sold out.
3. **User Profile.**
   1. Show booking status (booked / flight delayed / cancelled) derived from the flight's `Status`.
   2. Allow cancelling a ticket, restoring `SeatsAvailable` by 1.
   3. Angular: cancel action with a confirmation dialog.

**End-of-day acceptance:** sold-out and double-book attempts fail gracefully; admin edits propagate to the timetable; a cancelled ticket returns the seat.

## Day 3 — 2026-06-25 — Polish: Perf, Test, Security

**Goal:** make it correct, fast, and safe.

1. **Tests.**
   1. Backend unit tests for seat decrement, sold-out rejection, double-book guard, and cancel-restores-seat.
   2. Backend integration tests per endpoint (timetable query, booking POST, user bookings GET); extend the existing per-module test projects.
   3. Angular component/HTTP tests for the three pages.
2. **Performance.**
   1. Confirm timetable queries hit indexed columns and use pagination (no full scans).
   2. Remove any N+1 in the user-bookings join.
   3. Produce a production Angular build; check bundle size and that feature routes are lazy-loaded.
3. **Security.**
   1. Enforce auth: a user sees/cancels only their own bookings; only `airline-admin` can create/edit flights.
   2. Add matching Angular route guards (user vs admin).
   3. Re-run the ZAP pass (`pentest/`) and resolve any regressions.

**End-of-day acceptance:** green test suite; no obvious slow queries; authz holds on API and UI; ZAP shows no new findings.

## Day 4 — 2026-06-26 — Ship, Demo, Postmortem

**Goal:** deploy, show it working, capture lessons.

1. **Ship.**
   1. Deploy API + worker via the existing Bicep infra (`azure.yaml` / `main.bicep`) and host the Angular production build.
   2. Run a seeded migration/seed in the deployed environment and smoke-test all three slices end-to-end.
2. **Demo.**
   1. Walk the full path: admin-seeded timetable → user books a seat (count drops) → user tracks the ticket in their profile → cancel restores the seat.
3. **Postmortem.**
   1. Short retro doc: what worked, what was cut, follow-ups (e.g. real auth provider, per-cabin seat inventory).

**End-of-day acceptance:** deployed environment passes the demo script; retro committed under `docs/`.

---
