import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TicketService } from './ticket.service';
import { Ticket } from './ticket.models';

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

  // Cancel confirmation box.
  readonly confirming = signal(false);
  readonly cancelling = signal(false);

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
      error: () => {
        this.loading.set(false);
        this.error.set('Could not load your tickets (is the API on :5162?).');
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

  confirmCancel(): void {
    const ticket = this.selected();
    if (!ticket) return;

    this.cancelling.set(true);
    this.tickets.cancel(ticket.ticketId).subscribe({
      next: () => {
        this.cancelling.set(false);
        this.confirming.set(false);
        this.selected.set(null);
        this.load();
      },
      error: () => {
        this.cancelling.set(false);
        this.confirming.set(false);
        this.error.set('Could not cancel the ticket. Please try again.');
      },
    });
  }
}
