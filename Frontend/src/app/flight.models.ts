export type FlightDirection = 'Departure' | 'Arrival';

/** A row in a departures/arrivals list (matches the API's FlightListItem). */
export interface FlightListItem {
  flightCode: string;
  time: string;       // "HH:mm"
  airline: string;
  city: string;       // destination (departure) or origin (arrival)
  code: string;       // destination/origin IATA code
  airport: string;    // full name of the destination/origin airport
  direction: FlightDirection;
}

/** Full flight details (matches the API's FlightDetail). */
export interface FlightDetail {
  flightCode: string;
  airport: string;
  direction: FlightDirection;
  time: string;
  airlineCode: string;
  airline: string;
  counterpartAirport: string;
  counterpartCode: string;
  counterpartCity: string;
  duration: string;
}

/** A page of results (matches the API's PagedResult<T>). */
export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPrevious: boolean;
  hasNext: boolean;
}

export type Airport = 'BOM' | 'BLR' | 'PNQ' | 'LON' | 'DBX';
export const AIRPORTS: Airport[] = ['BOM', 'BLR', 'PNQ', 'LON', 'DBX'];

export interface AirportInfo {
  code: Airport;
  fullName: string;
  city: string;
  /** Accepted search terms (code, city, alternate city name, airport name). */
  aliases: string[];
}

export const AIRPORT_INFO: AirportInfo[] = [
  {
    code: 'BOM',
    fullName: 'Chhatrapati Shivaji Maharaj International Airport',
    city: 'Mumbai',
    aliases: ['BOM', 'Mumbai', 'Chhatrapati Shivaji Maharaj International Airport'],
  },
  {
    code: 'BLR',
    fullName: 'Kempegowda International Airport Bengaluru',
    city: 'Bengaluru',
    aliases: ['BLR', 'Bengaluru', 'Bangalore', 'Kempegowda International Airport Bengaluru'],
  },
  {
    code: 'PNQ',
    fullName: 'Pune International Airport',
    city: 'Pune',
    aliases: ['PNQ', 'Pune', 'Pune International Airport'],
  },
  {
    code: 'LON',
    fullName: 'London Heathrow Airport',
    city: 'London',
    aliases: ['LON', 'LHR', 'London', 'Heathrow', 'London Heathrow Airport'],
  },
  {
    code: 'DBX',
    fullName: 'Dubai International Airport',
    city: 'Dubai',
    aliases: ['DBX', 'DXB', 'Dubai', 'Dubai International Airport'],
  },
];

/** Full airport name for a code, e.g. BOM -> "Chhatrapati Shivaji Maharaj International Airport". */
export function airportFullName(code: string): string {
  return AIRPORT_INFO.find(a => a.code === code)?.fullName ?? code;
}

/** City for an airport code, e.g. LON -> "London". Empty string if unknown. */
export function airportCity(code: string): string {
  return AIRPORT_INFO.find(a => a.code === code)?.city ?? '';
}

/** Full name + city, e.g. PNQ -> "Pune International Airport, Pune". */
export function airportLabel(code: Airport): string {
  const a = AIRPORT_INFO.find(x => x.code === code);
  return a ? `${a.fullName}, ${a.city}` : code;
}

/** Resolves any accepted alias (case-insensitive) to an airport code, or null. */
export function resolveAirport(term: string): Airport | null {
  const t = term.trim().toLowerCase();
  if (!t) return null;
  const match = AIRPORT_INFO.find(a => a.aliases.some(alias => alias.toLowerCase() === t));
  return match?.code ?? null;
}

/** Every alias across all airports — feeds the served-airport autocomplete fields. */
export const ALL_AIRPORT_ALIASES: string[] = AIRPORT_INFO.flatMap(a => a.aliases);
