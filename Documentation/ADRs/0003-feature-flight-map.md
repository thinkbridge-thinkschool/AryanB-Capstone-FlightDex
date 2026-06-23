# ADR 0003 — Feature: Flight Directory

- **Status:** Rejected
- **Date:** 2026-06-22
- **Deciders:** Aryan Bhalerao
- **Tags:** feature, read-only

## Context

1. Users want to browse flights grouped by origin and destination airport, not just as a flat table.
2. The data needed (airport, city, route) already lives in the seeded `Location` and `FlightView` read model.

## Decision

1. Add a `GET /v1/directory` read endpoint that groups existing `FlightView` rows by origin/destination.
2. Reuse `Location` and `FlightView` as-is — no new aggregate and no coordinates or map geometry.
3. Render the grouped list on the client; no mapping library or tile provider.

## Consequences

1. Pure read feature with a small footprint — one projection endpoint over seeded data.
2. Presentation is a list/grouping rather than a geographic map.

## Alternatives considered

1. A geospatial map with coordinates, GeoJSON, and live aircraft — rejected; no geospatial scope.
2. A dedicated `Directory` module — rejected; it is just a view over `Locations` and `Timetable`.
