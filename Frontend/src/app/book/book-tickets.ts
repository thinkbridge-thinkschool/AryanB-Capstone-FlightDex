import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { FlightService } from '../flight.service';
import {
  AIRPORT_INFO, SERVED_AIRPORT_OPTIONS, Airport, FlightListItem,
  airportCity, airportFullName, airportLabel, airportOptionLabel, resolveAirport,
} from '../flight.models';
import { AuthService } from '../auth/auth.service';
import { TicketService } from '../tickets/ticket.service';
import { Ticket } from '../tickets/ticket.models';
import { Autocomplete, AutocompleteOption } from '../shared/autocomplete';
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

  // Search boxes. The *Text signals hold the displayed value (typed text or a picked
  // "Name [Code], City" label); the *Term signals hold what's resolved/sent (the code when a
  // suggestion is picked, else the raw text).
  readonly from = signal('');       // "From" display text
  readonly fromTerm = signal('');   // …resolved against the served airports
  readonly to = signal('');         // "To" display text
  readonly toTerm = signal('');     // …sent to the timetable search
  readonly date = signal('');
  // Optional departure-time window. `after`/`before` are HH:mm strings; the dropdown
  // that holds them toggles open via `timeFilterOpen`.
  readonly after = signal('');
  readonly before = signal('');
  readonly timeFilterOpen = signal(false);

  // Airport name/city helpers for the result route blocks.
  readonly airportCity = airportCity;
  readonly airportFullName = airportFullName;

  toggleTimeFilter(): void { this.timeFilterOpen.update(v => !v); }
  /** Short summary of the active time window for the dropdown button. */
  readonly timeFilterLabel = computed(() => {
    const a = this.after(), b = this.before();
    if (a && b) return `${a}–${b}`;
    if (a) return `After ${a}`;
    if (b) return `Before ${b}`;
    return 'Any time';
  });

  // "From" only allows the 5 served airports — kept in code.
  readonly originSuggestions = SERVED_AIRPORT_OPTIONS;
  // "To" can be any airport — loaded once from the Locations suggestion endpoint.
  readonly destinationSuggestions = signal<AutocompleteOption[]>([]);

  constructor() {
    this.flights.getAirportSuggestions().subscribe({
      next: list => this.destinationSuggestions.set(
        list.map(a => ({ label: airportOptionLabel(a.code, a.city), value: a.code }))),
      error: () => { /* leave suggestions empty if the cache is unavailable */ },
    });
  }

  // ---- From / To field handlers ----------------------------------------
  onFromInput(text: string): void { this.from.set(text); this.fromTerm.set(text); }
  onFromPicked(option: AutocompleteOption): void { this.from.set(option.label); this.fromTerm.set(option.value); }
  onToInput(text: string): void { this.to.set(text); this.toTerm.set(text); }
  onToPicked(option: AutocompleteOption): void { this.to.set(option.label); this.toTerm.set(option.value); }

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

  // Buy → fetch fare → fake payment gateway → book.
  readonly selectedFlight = signal<FlightListItem | null>(null);
  readonly priceReady = signal(false);   // fare "loaded" (placeholder, no real amount)
  readonly priceLoading = signal(false);
  readonly booking = signal(false);
  readonly bookError = signal<string | null>(null);
  readonly booked = signal<Ticket | null>(null);
  /** Fake payment-gateway state. */
  readonly paymentStage = signal<'none' | 'processing' | 'completed'>('none');

  /** Placeholder fare — shown verbatim, no amount is computed. */
  readonly priceLabel = 'INR XX,XXX';

  readonly passengerName = computed(() => {
    const u = this.auth.user();
    return u ? `${u.firstName} ${u.lastName}` : '';
  });

  search(): void {
    this.bookError.set(null);
    this.booked.set(null);
    this.selectedFlight.set(null);
    this.priceReady.set(false);
    this.paymentStage.set('none');

    const origin = resolveAirport(this.fromTerm());
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

    const range = { after: this.after().trim(), before: this.before().trim() };
    this.flights.getDepartures(origin, this.toTerm().trim(), range, 1, 100).subscribe({
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

  /** Buy a flight: open the confirmation box and fetch its fare. */
  buyFlight(flight: FlightListItem): void {
    this.bookError.set(null);
    this.selectedFlight.set(flight);
    this.fetchPrice();
  }

  /** Simulated fare lookup — short "network" delay, then the placeholder fare shows. */
  private fetchPrice(): void {
    this.priceReady.set(false);
    this.priceLoading.set(true);
    setTimeout(() => {
      this.priceReady.set(true);
      this.priceLoading.set(false);
    }, 600);
  }

  dismissConfirm(): void {
    if (this.booking() || this.paymentStage() !== 'none') return;
    this.selectedFlight.set(null);
    this.priceReady.set(false);
  }

  /** Confirm the fare → fake payment gateway (always "completed") → book the ticket. */
  proceedToPayment(): void {
    if (!this.priceReady()) return;
    this.bookError.set(null);
    this.paymentStage.set('processing');
    setTimeout(() => {
      this.paymentStage.set('completed');     // gateway always succeeds
      setTimeout(() => this.confirmBooking(), 900);
    }, 1500);
  }

  private confirmBooking(): void {
    const flight = this.selectedFlight();
    const origin = this.searchedOrigin();
    const originInfo = AIRPORT_INFO.find(a => a.code === origin);

    if (!flight || !originInfo || !this.date()) {
      this.paymentStage.set('none');
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
        this.paymentStage.set('none');
        this.selectedFlight.set(null);
        this.priceReady.set(false);
        this.booked.set(ticket);
        this.results.set(null);
      },
      error: (err: HttpErrorResponse) => {
        this.booking.set(false);
        this.paymentStage.set('none');
        this.bookError.set(httpErrorMessage(err, 'Could not book the ticket. Please try again.'));
      },
    });
  }
}
