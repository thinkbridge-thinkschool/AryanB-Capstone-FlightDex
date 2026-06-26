import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { FlightService } from '../flight.service';
import {
  AIRPORT_INFO, SERVED_AIRPORT_OPTIONS, Airport, FlightDetail, FlightListItem,
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

  // Flight detail pop-up — same data as the Timetable "details" view.
  readonly detail = signal<FlightDetail | null>(null);
  readonly detailLoading = signal(false);

  // Booking — a fake staged interface that still creates the real ticket behind the scenes.
  readonly selectedFlight = signal<FlightListItem | null>(null);
  readonly booking = signal(false);
  readonly booked = signal<Ticket | null>(null);
  /** Stages of the booking flow: confirm details → fake redirect → confirming → confirmed. */
  readonly bookStage = signal<'none' | 'confirm' | 'redirect' | 'confirming' | 'confirmed'>('none');

  readonly passengerName = computed(() => {
    const u = this.auth.user();
    return u ? `${u.firstName} ${u.lastName}` : '';
  });

  search(): void {
    this.booked.set(null);
    this.selectedFlight.set(null);
    this.detail.set(null);
    this.bookStage.set('none');

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
    this.flights.getDepartures(origin, this.toTerm().trim(), '', range, 1, 100).subscribe({
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

  // ---- Flight details pop-up -------------------------------------------

  /** Open the details pop-up for a flight (same data/shape as the Timetable view). */
  openDetail(flightCode: string): void {
    this.detailLoading.set(true);
    this.detail.set(null);
    this.flights.getDetail(flightCode).subscribe({
      next: d => { this.detail.set(d); this.detailLoading.set(false); },
      error: () => { this.detailLoading.set(false); },
    });
  }

  closeDetail(): void {
    this.detail.set(null);
    this.detailLoading.set(false);
  }

  /** Time as 12-hour with a meridiem, e.g. "14:30" -> "02:30 PM". */
  formatTime(time: string): string {
    const [h, m = '00'] = time.split(':');
    const hour24 = Number(h);
    if (Number.isNaN(hour24)) return time;
    const period = hour24 >= 12 ? 'PM' : 'AM';
    const hour12 = hour24 % 12 || 12;
    return `${String(hour12).padStart(2, '0')}:${m} ${period}`;
  }

  // ---- Booking — fake staged interface ---------------------------------

  /** Book a flight: first show the details-confirmation box. */
  bookFlight(flight: FlightListItem): void {
    this.booked.set(null);
    this.selectedFlight.set(flight);
    this.bookStage.set('confirm');
  }

  /** Confirmed the details: run the fake "redirect → confirming → confirmed" interface. */
  proceedBooking(): void {
    const flight = this.selectedFlight();
    if (!flight) return;
    this.bookStage.set('redirect');
    setTimeout(() => {
      this.bookStage.set('confirming');
      this.createTicket(flight);
    }, 1400);
  }

  /** Dismiss the confirmation box without booking (keeps the results list). */
  cancelBook(): void {
    this.bookStage.set('none');
    this.selectedFlight.set(null);
  }

  /** Creates the real ticket in the background; the interface always ends in "Confirmed". */
  private createTicket(flight: FlightListItem): void {
    const originInfo = AIRPORT_INFO.find(a => a.code === this.searchedOrigin());
    if (!originInfo || !this.date()) {
      setTimeout(() => this.bookStage.set('confirmed'), 900);
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
        this.booked.set(ticket);
        setTimeout(() => this.bookStage.set('confirmed'), 900);
      },
      // The fake interface always reaches "Booking Confirmed", even if the API call fails.
      error: () => {
        this.booking.set(false);
        setTimeout(() => this.bookStage.set('confirmed'), 900);
      },
    });
  }

  closeBook(): void {
    this.bookStage.set('none');
    this.selectedFlight.set(null);
    this.results.set(null);
  }
}
