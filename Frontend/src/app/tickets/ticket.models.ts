/** A booked ticket (matches the API's TicketDto). */
export interface Ticket {
  ticketId: number;
  date: string;   // "yyyy-MM-dd"
  time: string;   // "HH:mm"
  originCode: string;
  originAirport: string;
  originCity: string;
  destinationCode: string;
  destinationAirport: string;
  destinationCity: string;
  firstName: string;
  lastName: string;
  age: number;
}

/** An airport reference sent when booking. */
export interface AirportRef {
  code: string;
  airport: string;
  city: string;
}

/** Payload for POST /ticket. */
export interface BookTicketRequest {
  date: string;   // "yyyy-MM-dd"
  time: string;   // "HH:mm"
  origin: AirportRef;
  destination: AirportRef;
}
