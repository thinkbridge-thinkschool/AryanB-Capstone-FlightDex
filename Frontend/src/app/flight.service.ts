import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { Airport, FlightDetail, FlightListItem, PagedResult } from './flight.models';

/**
 * Base URL of the FlightDex API. Empty = same-origin: in the Docker setup nginx serves
 * the app and reverse-proxies "/flight" to the API, so relative URLs avoid CORS.
 */
export const API_BASE_URL = '';

export interface TimeRange {
  after?: string;   // "HHMM"
  before?: string;  // "HHMM"
}

@Injectable({ providedIn: 'root' })
export class FlightService {
  private readonly http = inject(HttpClient);

  /** Departures from an airport, optionally to a destination and within a time window. */
  getDepartures(
    at: Airport, to: string, time: TimeRange, page: number, pageSize: number,
  ): Observable<PagedResult<FlightListItem>> {
    let params = new HttpParams().set('at', at).set('page', page).set('pageSize', pageSize);
    // `to` (even empty) selects the departures view on the server. May be a code or city.
    params = params.set('to', to.trim());
    if (time.after) params = params.set('deptTime_After', time.after);
    if (time.before) params = params.set('deptTime_Before', time.before);

    return this.http.get<PagedResult<FlightListItem>>(`${API_BASE_URL}/flight`, { params });
  }

  /** Arrivals at an airport, optionally from an origin and within a time window. */
  getArrivals(
    at: Airport, from: string, time: TimeRange, page: number, pageSize: number,
  ): Observable<PagedResult<FlightListItem>> {
    let params = new HttpParams().set('at', at).set('page', page).set('pageSize', pageSize);
    // `from` (even empty) selects the arrivals view on the server. May be a code or city.
    params = params.set('from', from.trim());
    if (time.after) params = params.set('arrTime_After', time.after);
    if (time.before) params = params.set('arrTime_Before', time.before);

    return this.http.get<PagedResult<FlightListItem>>(`${API_BASE_URL}/flight`, { params });
  }

  /**
   * Airport search suggestions (codes, airport names and cities) served from the Redis
   * cache. Does not touch the timetable database — only the actual search does.
   */
  getAirportSuggestions(): Observable<string[]> {
    return this.http.get<string[]>(`${API_BASE_URL}/airports/suggestions`);
  }

  /** Full details for a flight code (returns one record, or the first if several match). */
  getDetail(flightCode: string): Observable<FlightDetail> {
    return this.http
      .get<FlightDetail | FlightDetail[]>(`${API_BASE_URL}/flight/${encodeURIComponent(flightCode)}`)
      .pipe(map(r => (Array.isArray(r) ? r[0] : r)));
  }
}
