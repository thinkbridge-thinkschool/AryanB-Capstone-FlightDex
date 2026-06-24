import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { TicketService } from './ticket.service';
import { Ticket } from './ticket.models';

@Component({
  selector: 'app-my-tickets',
  imports: [RouterLink, FormsModule],
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

  // Reschedule (edit date/time) box.
  readonly rescheduling = signal(false);
  readonly saving = signal(false);
  readonly editError = signal<string | null>(null);
  readonly editDate = signal('');
  readonly editTime = signal('');

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

  askReschedule(): void {
    const ticket = this.selected();
    if (!ticket) return;
    // Prefill the form with the ticket's current date/time.
    this.editDate.set(ticket.date);
    this.editTime.set(ticket.time);
    this.editError.set(null);
    this.rescheduling.set(true);
  }

  dismissReschedule(): void {
    if (!this.saving()) this.rescheduling.set(false);
  }

  confirmReschedule(): void {
    const ticket = this.selected();
    if (!ticket) return;

    const date = this.editDate().trim();
    const time = this.editTime().trim();
    if (!date || !time) {
      this.editError.set('Pick a new date and time.');
      return;
    }

    this.saving.set(true);
    this.editError.set(null);
    this.tickets.update(ticket.ticketId, { date, time }).subscribe({
      next: updated => {
        this.saving.set(false);
        this.rescheduling.set(false);
        // Reflect the change in place without a full reload.
        this.items.update(list => list.map(t => (t.ticketId === updated.ticketId ? updated : t)));
        this.selected.set(updated);
      },
      error: () => {
        this.saving.set(false);
        this.editError.set('Could not reschedule the ticket. Please try again.');
      },
    });
  }
}
