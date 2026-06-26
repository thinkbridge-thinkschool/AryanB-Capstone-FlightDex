import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { Airport, AirportSuggestion, FlightDetail, FlightListItem, PagedResult } from './flight.models';

// All API calls use same-origin relative paths: in the Docker setup nginx serves the
// app and reverse-proxies "/flight" etc. to the API, so relative URLs avoid CORS.

export interface TimeRange {
  after?: string;   // "HHMM"
  before?: string;  // "HHMM"
}

@Injectable({ providedIn: 'root' })
export class FlightService {
  private readonly http = inject(HttpClient);

  /** Departures from an airport, optionally to a destination, by flight code and within a time window. */
  getDepartures(
    at: Airport, to: string, flightCode: string, time: TimeRange, page: number, pageSize: number,
  ): Observable<PagedResult<FlightListItem>> {
    let params = new HttpParams().set('at', at).set('page', page).set('pageSize', pageSize);
    // `to` (even empty) selects the departures view on the server. May be a code or city.
    params = params.set('to', to.trim());
    if (flightCode.trim()) params = params.set('flightCode', flightCode.trim());
    if (time.after) params = params.set('deptTime_After', time.after);
    if (time.before) params = params.set('deptTime_Before', time.before);

    return this.http.get<PagedResult<FlightListItem>>('/flight', { params });
  }

  /** Arrivals at an airport, optionally from an origin, by flight code and within a time window. */
  getArrivals(
    at: Airport, from: string, flightCode: string, time: TimeRange, page: number, pageSize: number,
  ): Observable<PagedResult<FlightListItem>> {
    let params = new HttpParams().set('at', at).set('page', page).set('pageSize', pageSize);
    // `from` (even empty) selects the arrivals view on the server. May be a code or city.
    params = params.set('from', from.trim());
    if (flightCode.trim()) params = params.set('flightCode', flightCode.trim());
    if (time.after) params = params.set('arrTime_After', time.after);
    if (time.before) params = params.set('arrTime_Before', time.before);

    return this.http.get<PagedResult<FlightListItem>>('/flight', { params });
  }

  /**
   * Airport search suggestions (code, name and city per airport) served from the
   * Locations lookup table. Does not touch the Flights table — only the actual search does.
   */
  getAirportSuggestions(): Observable<AirportSuggestion[]> {
    return this.http.get<AirportSuggestion[]>('/airports/suggestions');
  }

  /** Full details for a flight code (returns one record, or the first if several match). */
  getDetail(flightCode: string): Observable<FlightDetail> {
    return this.http
      .get<FlightDetail | FlightDetail[]>(`/flight/${encodeURIComponent(flightCode)}`)
      .pipe(map(r => (Array.isArray(r) ? r[0] : r)));
  }
}
