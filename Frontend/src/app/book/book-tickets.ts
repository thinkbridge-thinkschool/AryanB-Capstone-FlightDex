import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { FlightService } from '../flight.service';
import {
  AIRPORT_INFO, ALL_AIRPORT_ALIASES, Airport, FlightListItem, airportLabel, resolveAirport,
} from '../flight.models';
import { AuthService } from '../auth/auth.service';
import { TicketService } from '../tickets/ticket.service';
import { Ticket } from '../tickets/ticket.models';

@Component({
  selector: 'app-book-tickets',
  imports: [FormsModule, RouterLink],
  templateUrl: './book-tickets.html',
  styleUrl: './book-tickets.css',
})
export class BookTickets {
  private readonly flights = inject(FlightService);
  private readonly tickets = inject(TicketService);
  private readonly auth = inject(AuthService);

  readonly today = new Date().toISOString().slice(0, 10);

  // Search boxes (free text, resolved against the served airports / timetable).
  readonly from = signal('');
  readonly to = signal('');
  readonly date = signal(this.today);

  // Suggestions for the search-box datalists.
  readonly originSuggestions = ALL_AIRPORT_ALIASES;
  readonly destinationSuggestions = signal<string[]>([]);

  // Flight list (results of a search).
  readonly results = signal<FlightListItem[] | null>(null);
  readonly searching = signal(false);
  readonly searchError = signal<string | null>(null);
  /** The served airport the current results were searched from. */
  readonly searchedOrigin = signal<Airport | null>(null);
  readonly searchedOriginLabel = computed(() => {
    const o = this.searchedOrigin();
    return o ? airportLabel(o) : '';
  });

  // Confirmation + booking.
  readonly selectedFlight = signal<FlightListItem | null>(null);
  readonly booking = signal(false);
  readonly bookError = signal<string | null>(null);
  readonly booked = signal<Ticket | null>(null);

  readonly passengerName = computed(() => {
    const u = this.auth.user();
    return u ? `${u.firstName} ${u.lastName}` : '';
  });

  /** As the origin is typed, load that airport's destinations for the "To" suggestions. */
  onFromChange(value: string): void {
    this.from.set(value);
    const origin = resolveAirport(value);
    if (!origin) {
      this.destinationSuggestions.set([]);
      return;
    }
    this.flights.getDepartures(origin, '', {}, 1, 100).subscribe({
      next: page => {
        const cities = [...new Set(page.items.map(f => f.city))].sort((a, b) => a.localeCompare(b));
        this.destinationSuggestions.set(cities);
      },
      error: () => this.destinationSuggestions.set([]),
    });
  }

  search(): void {
    this.bookError.set(null);
    this.booked.set(null);

    const origin = resolveAirport(this.from());
    if (!origin) {
      this.searchError.set('Origin must be a served airport: BLR, BOM or PNQ (or its city).');
      this.results.set(null);
      return;
    }
    if (!this.date()) {
      this.searchError.set('Choose a travel date.');
      return;
    }

    this.searching.set(true);
    this.searchError.set(null);
    this.results.set(null);

    this.flights.getDepartures(origin, this.to().trim(), {}, 1, 100).subscribe({
      next: page => {
        this.searchedOrigin.set(origin);
        this.results.set(page.items);
        this.searching.set(false);
      },
      error: () => {
        this.searching.set(false);
        this.searchError.set('Could not load flights (is the API on :5162?).');
      },
    });
  }

  /** Open the confirmation box for a chosen flight. */
  selectFlight(flight: FlightListItem): void {
    this.bookError.set(null);
    this.selectedFlight.set(flight);
  }

  dismissConfirm(): void {
    if (!this.booking()) this.selectedFlight.set(null);
  }

  confirmBooking(): void {
    const flight = this.selectedFlight();
    const origin = this.searchedOrigin();
    const originInfo = AIRPORT_INFO.find(a => a.code === origin);

    if (!flight || !originInfo || !this.date()) {
      this.bookError.set('Something is missing — please search and pick a flight again.');
      return;
    }

    this.booking.set(true);
    this.tickets.book({
      date: this.date(),
      time: flight.time,
      origin: { code: originInfo.code, airport: originInfo.fullName, city: originInfo.city },
      destination: { code: flight.code, airport: flight.airport, city: flight.city },
    }).subscribe({
      next: ticket => {
        this.booking.set(false);
        this.selectedFlight.set(null);
        this.booked.set(ticket);
        this.results.set(null);
      },
      error: (err: HttpErrorResponse) => {
        this.booking.set(false);
        this.bookError.set(err.status === 0
          ? 'Could not reach the server. Is the API running on :5162?'
          : (err.error?.detail ?? 'Could not book the ticket. Please try again.'));
      },
    });
  }
}
