import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { FlightService, TimeRange } from '../flight.service';
import {
  AIRPORT_INFO, ALL_AIRPORT_ALIASES, Airport, FlightDetail, FlightListItem,
  PagedResult, airportLabel, resolveAirport,
} from '../flight.models';

/** Which result box(es) are shown. */
type View = 'both' | 'departures' | 'arrivals';

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
  imports: [FormsModule],
  templateUrl: './timetable.html',
  styleUrl: './timetable.css',
})
export class Timetable {
  private readonly flights = inject(FlightService);

  readonly airportInfo = AIRPORT_INFO;
  readonly aliasSuggestions = ALL_AIRPORT_ALIASES;

  // "You are currently at" search box. Default airport: Pune (PNQ).
  readonly airportSearch = signal('');
  readonly at = signal<Airport>('PNQ');
  readonly airportError = signal<string | null>(null);

  readonly view = signal<View>('both');

  // Search bar fields (accept a destination/origin code OR a city name).
  readonly departTo = signal('');     // "Departing To:"
  readonly arriveFrom = signal('');   // "Arriving From:"

  // "More Search Options" dropdown (time filters).
  readonly moreOptions = signal(false);
  readonly deptAfter = signal('');
  readonly deptBefore = signal('');
  readonly arrAfter = signal('');
  readonly arrBefore = signal('');

  // The two result boxes.
  readonly departures = signal<BoxState>(emptyBox());
  readonly arrivals = signal<BoxState>(emptyBox());

  // Detail modal.
  readonly selected = signal<FlightDetail | null>(null);
  readonly detailLoading = signal(false);

  readonly currentAirportLabel = computed(() => airportLabel(this.at()));
  readonly resultsTitle = computed(() => {
    const label = this.currentAirportLabel();
    switch (this.view()) {
      case 'departures': return `Departures at ${label}`;
      case 'arrivals': return `Arrivals at ${label}`;
      default: return `Departures and Arrivals at ${label}`;
    }
  });

  constructor() {
    this.loadDepartures();
    this.loadArrivals();
  }

  // ---- Airport selection ------------------------------------------------

  applyAirportSearch(): void {
    const code = resolveAirport(this.airportSearch());
    if (!code) {
      this.airportError.set('Try a code (BOM/BLR/PNQ), city, or airport name.');
      return;
    }
    this.airportError.set(null);
    this.at.set(code);
    this.showBoth(); // reset to the default both-boxes view for the new airport
  }

  // ---- Search bar -------------------------------------------------------

  toggleMoreOptions(): void {
    this.moreOptions.update(v => !v);
  }

  /** Runs the search and shows only the relevant box(es). */
  doSearch(): void {
    const wantsDep = !!(this.departTo().trim() || this.deptAfter().trim() || this.deptBefore().trim());
    const wantsArr = !!(this.arriveFrom().trim() || this.arrAfter().trim() || this.arrBefore().trim());

    if (wantsDep && !wantsArr) this.view.set('departures');
    else if (wantsArr && !wantsDep) this.view.set('arrivals');
    else this.view.set('both');

    this.departures.update(b => ({ ...b, page: 1 }));
    this.arrivals.update(b => ({ ...b, page: 1 }));

    const v = this.view();
    if (v === 'both' || v === 'departures') this.loadDepartures();
    if (v === 'both' || v === 'arrivals') this.loadArrivals();
  }

  /** Clear filters and show both boxes for the current airport. */
  showBoth(): void {
    this.departTo.set(''); this.arriveFrom.set('');
    this.deptAfter.set(''); this.deptBefore.set('');
    this.arrAfter.set(''); this.arrBefore.set('');
    this.departures.update(b => ({ ...b, page: 1 }));
    this.arrivals.update(b => ({ ...b, page: 1 }));
    this.view.set('both');
    this.loadDepartures();
    this.loadArrivals();
  }

  // ---- Loading ----------------------------------------------------------

  private loadDepartures(): void {
    this.departures.update(b => ({ ...b, loading: true, error: null }));
    const range: TimeRange = { after: this.deptAfter().trim(), before: this.deptBefore().trim() };
    this.flights.getDepartures(this.at(), this.departTo(), range, this.departures().page, PAGE_SIZE)
      .subscribe({
        next: r => this.departures.update(b => ({ ...b, result: r, loading: false })),
        error: () => this.departures.update(b => ({
          ...b, result: null, loading: false, error: 'Could not load departures (is the API on :5162?).',
        })),
      });
  }

  private loadArrivals(): void {
    this.arrivals.update(b => ({ ...b, loading: true, error: null }));
    const range: TimeRange = { after: this.arrAfter().trim(), before: this.arrBefore().trim() };
    this.flights.getArrivals(this.at(), this.arriveFrom(), range, this.arrivals().page, PAGE_SIZE)
      .subscribe({
        next: r => this.arrivals.update(b => ({ ...b, result: r, loading: false })),
        error: () => this.arrivals.update(b => ({
          ...b, result: null, loading: false, error: 'Could not load arrivals (is the API on :5162?).',
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
