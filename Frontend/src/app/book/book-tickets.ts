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
import { Autocomplete } from '../shared/autocomplete';
import { ShowPickerDirective } from '../shared/show-picker.directive';
import { httpErrorMessage } from '../shared/http-errors';

@Component({
  selector: 'app-book-tickets',
  imports: [FormsModule, RouterLink, Autocomplete, ShowPickerDirective],
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
  readonly date = signal('');

  // The date input swaps text<->date so the custom placeholder shows when empty.
  readonly dateType = signal<'text' | 'date'>('text');

  // "From" only allows the 5 served airports — kept in code (code/name/city aliases).
  readonly originSuggestions = ALL_AIRPORT_ALIASES;
  // "To" can be any airport — loaded once from the Locations suggestion endpoint.
  readonly destinationSuggestions = signal<string[]>([]);

  constructor() {
    this.flights.getAirportSuggestions().subscribe({
      next: list => this.destinationSuggestions.set(list),
      error: () => { /* leave suggestions empty if the cache is unavailable */ },
    });
  }

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

  /** When the date field loses focus while empty, revert to text so the placeholder shows. */
  onDateBlur(): void {
    if (!this.date()) this.dateType.set('text');
  }

  search(): void {
    this.bookError.set(null);
    this.booked.set(null);

    const origin = resolveAirport(this.from());
    if (!origin) {
      this.searchError.set('Origin must be a served airport: BLR, BOM, PNQ, LON or DBX (or its city).');
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
      error: (err: HttpErrorResponse) => {
        this.searching.set(false);
        this.searchError.set(httpErrorMessage(err, 'Could not load flights. Please try again.'));
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
        this.bookError.set(httpErrorMessage(err, 'Could not book the ticket. Please try again.'));
      },
    });
  }
}
