# ADR 0002 — Feature: Ticket Booking

- **Status:** Accepted
- **Date:** 2026-06-22
- **Deciders:** Aryan Bhalerao
- **Tags:** feature, transactional, new-module

## Context

1. Every feature so far is read-only; Ticket Booking is the first feature that writes data.
2. A booking links a user to a flight and moves through a simple lifecycle (`Created / Confirmed / Cancelled`).

## Decision

1. Add a `Booking` module with a `Booking` entity that references a flight by `FlightId`.
2. Expose CRUD endpoints (`POST` / `GET` / `PUT` / `DELETE /v1/bookings`) backed by Azure SQL.
3. Seed sample bookings from the database, consistent with the rest of the app.

## Consequences

1. Introduces the project's first write endpoints while staying within the existing module layout.
2. No payments, inventory locking, or sagas — booking is treated as plain CRUD.

## Alternatives considered

1. External payment provider with a saga and seat-inventory concurrency — rejected as out of scope.
2. A separate booking microservice — deferred; the modular monolith is sufficient.
