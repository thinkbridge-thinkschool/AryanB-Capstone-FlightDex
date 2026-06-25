import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { FlightService, TimeRange } from '../flight.service';
import {
  SERVED_AIRPORT_OPTIONS, Airport, FlightDetail, FlightListItem,
  PagedResult, airportCity, airportFullName, airportOptionLabel, resolveAirport,
} from '../flight.models';
import { HttpErrorResponse } from '@angular/common/http';
import { Autocomplete, AutocompleteOption } from '../shared/autocomplete';
import { ShowPickerDirective } from '../shared/show-picker.directive';
import { httpErrorMessage } from '../shared/http-errors';

const PAGE_SIZE = 30;

/** State for one result box (departures or arrivals). */
interface BoxState {
  result: PagedResult<FlightListItem> | null;
  page: number;
  loading: boolean;
  error: string | null;
}

const emptyBox = (): BoxState => ({ result: null, page: 1, loading: false, error: null });

@Component({
  selector: 'app-timetable',
  imports: [FormsModule, Autocomplete, ShowPickerDirective],
  templateUrl: './timetable.html',
  styleUrl: './timetable.css',
})
export class Timetable {
  private readonly flights = inject(FlightService);

  /** Served-airport options (the 5 airports) for the "Change Airport" box — kept in code. */
  readonly airportOptions = SERVED_AIRPORT_OPTIONS;
  /** Counterpart (any-airport) options for the search boxes — from the Locations cache. */
  readonly suggestions = signal<AutocompleteOption[]>([]);

  /** Exposed for the detail modal. */
  readonly airportFullName = airportFullName;
  readonly airportCity = airportCity;

  // "Timetable for" airport selector. Default airport: Pune (PNQ).
  readonly airportSearch = signal('');
  readonly at = signal<Airport>('PNQ');
  readonly airportError = signal<string | null>(null);
  /** Whether the "Change Airport" dropdown is open. */
  readonly changeAirportOpen = signal(false);

  // Per-box search fields. The *Text signals hold what's shown in the input (a typed string
  // or a picked "Name [Code], City" label); the *Term signals hold the value actually sent to
  // the API (the airport code when a suggestion is picked, else the raw text).
  readonly departTo = signal('');         // Departures box: "Departing To:" display text
  readonly departToTerm = signal('');     // …the search term sent to the API
  readonly arriveFrom = signal('');       // Arrivals box: "Arriving From:" display text
  readonly arriveFromTerm = signal('');   // …the search term sent to the API

  // Per-box time filters.
  readonly deptAfter = signal('');
  readonly deptBefore = signal('');
  readonly arrAfter = signal('');
  readonly arrBefore = signal('');

  // Per-box search dropdowns.
  readonly depSearchOpen = signal(false);
  readonly arrSearchOpen = signal(false);

  // The two result boxes.
  readonly departures = signal<BoxState>(emptyBox());
  readonly arrivals = signal<BoxState>(emptyBox());

  // Detail modal.
  readonly selected = signal<FlightDetail | null>(null);
  readonly detailLoading = signal(false);

  /** Display text for the airport field, e.g. "Pune International Airport [PNQ], Pune". */
  readonly airportDisplay = computed(() => {
    const code = this.at();
    return `${airportFullName(code)} [${code}], ${airportCity(code)}`;
  });

  constructor() {
    this.loadDepartures();
    this.loadArrivals();
    this.flights.getAirportSuggestions().subscribe({
      next: list => this.suggestions.set(
        list.map(a => ({ label: airportOptionLabel(a.code, a.city), value: a.code }))),
      error: () => { /* leave suggestions empty if the cache is unavailable */ },
    });
  }

  // ---- Airport selection ------------------------------------------------

  /** Open/close the "Change Airport" dropdown. */
  toggleChangeAirport(): void {
    this.changeAirportOpen.update(open => {
      if (!open) { this.airportSearch.set(''); this.airportError.set(null); }
      return !open;
    });
  }

  /** Picked an airport from the suggestion list. */
  onAirportPicked(option: AutocompleteOption): void {
    this.airportSearch.set(option.value);
    this.applyAirportSearch();
  }

  applyAirportSearch(): void {
    const code = resolveAirport(this.airportSearch());
    if (!code) {
      this.airportError.set('Try a code (BOM/BLR/PNQ/LON/DBX), city, or airport name.');
      return;
    }
    this.airportError.set(null);
    this.changeAirportOpen.set(false);
    this.at.set(code);

    // New airport: clear any per-box searches and reload both boxes from the top.
    this.departTo.set(''); this.departToTerm.set('');
    this.arriveFrom.set(''); this.arriveFromTerm.set('');
    this.deptAfter.set(''); this.deptBefore.set('');
    this.arrAfter.set(''); this.arrBefore.set('');
    this.depSearchOpen.set(false); this.arrSearchOpen.set(false);
    this.departures.update(b => ({ ...b, page: 1 }));
    this.arrivals.update(b => ({ ...b, page: 1 }));
    this.loadDepartures();
    this.loadArrivals();
  }

  // ---- Per-box search ---------------------------------------------------

  toggleDepSearch(): void { this.depSearchOpen.update(v => !v); }
  toggleArrSearch(): void { this.arrSearchOpen.update(v => !v); }

  /** Free text typed into "Departing To:" — both the display and the search term are the text. */
  onDepartToInput(text: string): void { this.departTo.set(text); this.departToTerm.set(text); }
  onArriveFromInput(text: string): void { this.arriveFrom.set(text); this.arriveFromTerm.set(text); }

  /** Picked a destination suggestion: show its label, search by its code. */
  onDepartToPicked(option: AutocompleteOption): void {
    this.departTo.set(option.label);
    this.departToTerm.set(option.value);
    this.searchDepartures();
  }

  /** Picked an origin suggestion: show its label, search by its code. */
  onArriveFromPicked(option: AutocompleteOption): void {
    this.arriveFrom.set(option.label);
    this.arriveFromTerm.set(option.value);
    this.searchArrivals();
  }

  /** Search departures with the current filters (resets to the first page). */
  searchDepartures(): void {
    this.departures.update(b => ({ ...b, page: 1 }));
    this.loadDepartures();
  }

  /** Search arrivals with the current filters (resets to the first page). */
  searchArrivals(): void {
    this.arrivals.update(b => ({ ...b, page: 1 }));
    this.loadArrivals();
  }

  clearDepSearch(): void {
    this.departTo.set(''); this.departToTerm.set('');
    this.deptAfter.set(''); this.deptBefore.set('');
    this.searchDepartures();
  }

  clearArrSearch(): void {
    this.arriveFrom.set(''); this.arriveFromTerm.set('');
    this.arrAfter.set(''); this.arrBefore.set('');
    this.searchArrivals();
  }

  // ---- Loading ----------------------------------------------------------

  private loadDepartures(): void {
    this.departures.update(b => ({ ...b, loading: true, error: null }));
    const range: TimeRange = { after: this.deptAfter().trim(), before: this.deptBefore().trim() };
    this.flights.getDepartures(this.at(), this.departToTerm(), range, this.departures().page, PAGE_SIZE)
      .subscribe({
        next: r => this.departures.update(b => ({ ...b, result: r, loading: false })),
        error: (err: HttpErrorResponse) => this.departures.update(b => ({
          ...b, result: null, loading: false,
          error: httpErrorMessage(err, 'Could not load departures. Please try again.'),
        })),
      });
  }

  private loadArrivals(): void {
    this.arrivals.update(b => ({ ...b, loading: true, error: null }));
    const range: TimeRange = { after: this.arrAfter().trim(), before: this.arrBefore().trim() };
    this.flights.getArrivals(this.at(), this.arriveFromTerm(), range, this.arrivals().page, PAGE_SIZE)
      .subscribe({
        next: r => this.arrivals.update(b => ({ ...b, result: r, loading: false })),
        error: (err: HttpErrorResponse) => this.arrivals.update(b => ({
          ...b, result: null, loading: false,
          error: httpErrorMessage(err, 'Could not load arrivals. Please try again.'),
        })),
      });
  }

  goToDeparturePage(p: number): void {
    const total = this.departures().result?.totalPages ?? 1;
    if (p < 1 || p > total) return;
    this.departures.update(b => ({ ...b, page: p }));
    this.loadDepartures();
  }

  goToArrivalPage(p: number): void {
    const total = this.arrivals().result?.totalPages ?? 1;
    if (p < 1 || p > total) return;
    this.arrivals.update(b => ({ ...b, page: p }));
    this.loadArrivals();
  }

  // ---- Detail modal -----------------------------------------------------

  openDetail(flightCode: string): void {
    this.detailLoading.set(true);
    this.selected.set(null);
    this.flights.getDetail(flightCode).subscribe({
      next: d => { this.selected.set(d); this.detailLoading.set(false); },
      error: () => { this.detailLoading.set(false); },
    });
  }

  closeDetail(): void {
    this.selected.set(null);
  }
}
