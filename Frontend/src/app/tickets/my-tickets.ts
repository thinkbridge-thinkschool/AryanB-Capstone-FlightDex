import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { TicketService } from './ticket.service';
import { Ticket } from './ticket.models';
import { httpErrorMessage } from '../shared/http-errors';

@Component({
  selector: 'app-my-tickets',
  imports: [RouterLink],
  templateUrl: './my-tickets.html',
  styleUrl: './my-tickets.css',
})
export class MyTickets {
  private readonly tickets = inject(TicketService);

  readonly items = signal<Ticket[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

  readonly selected = signal<Ticket | null>(null);

  // Cancel confirmation box, then a fake staged interface.
  readonly confirming = signal(false);
  /** Stages of the fake cancellation interface. */
  readonly cancelStage = signal<'none' | 'cancelling' | 'confirming' | 'cancelled'>('none');

  constructor() {
    this.load();
  }

  private load(): void {
    this.loading.set(true);
    this.error.set(null);
    this.tickets.getMine().subscribe({
      next: list => {
        this.items.set(list);
        // Keep the current selection if it still exists, else pick the first.
        const current = this.selected();
        const stillThere = current && list.find(t => t.ticketId === current.ticketId);
        this.selected.set(stillThere ?? list[0] ?? null);
        this.loading.set(false);
      },
      error: (err: HttpErrorResponse) => {
        this.loading.set(false);
        this.error.set(httpErrorMessage(err, 'Could not load your tickets. Please try again.'));
      },
    });
  }

  select(ticket: Ticket): void {
    this.selected.set(ticket);
  }

  askCancel(): void {
    if (this.selected()) this.confirming.set(true);
  }

  dismissCancel(): void {
    this.confirming.set(false);
  }

  /** Confirmed the cancel: close the prompt and run the fake "cancelling → confirming → cancelled" flow. */
  confirmCancel(): void {
    const ticket = this.selected();
    if (!ticket) return;

    this.confirming.set(false);
    this.cancelStage.set('cancelling');
    setTimeout(() => {
      this.cancelStage.set('confirming');
      this.tickets.cancel(ticket.ticketId).subscribe({
        // The fake interface always reaches "Booking Cancelled", even if the API call fails.
        next: () => setTimeout(() => this.cancelStage.set('cancelled'), 900),
        error: () => setTimeout(() => this.cancelStage.set('cancelled'), 900),
      });
    }, 1200);
  }

  /** Close the cancellation interface and refresh the list. */
  closeCancel(): void {
    this.cancelStage.set('none');
    this.selected.set(null);
    this.load();
  }
}
