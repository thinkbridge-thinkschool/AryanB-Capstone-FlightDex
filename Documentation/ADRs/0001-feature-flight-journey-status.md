# ADR 0001 — Feature: Flight Journey Status

- **Status:** Rejected
- **Date:** 2026-06-22
- **Deciders:** Aryan Bhalerao
- **Tags:** feature, frontend-heavy, read-only

## Context

1. The timetable shows departure and arrival times but gives no sense of where a flight is *along* its journey.
2. Flight Journey Status would render a **progress bar** per flight, showing how far it has travelled between origin and destination.

## Decision

1. Compute a journey-progress percentage from departure/arrival times and the current time.
2. Display it as a progress bar on the timetable and flight-detail pages.

## Consequences

1. Progress is only an interpolation between scheduled times — it does not reflect real position, delays, or actual movement.
2. A purely time-based bar can mislead users when a flight is delayed or diverted, since the data to correct it does not exist.

## Alternatives considered

1. Drive the bar from a real position/status feed — rejected; it requires live tracking, which is out of scope for a seeded CRUD app.
2. Show only textual scheduled times (no bar) — this is the existing behavior and remains sufficient, so the feature was rejected.
