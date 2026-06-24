import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BookTicketRequest, Ticket, UpdateTicketRequest } from './ticket.models';

/**
 * Booking, listing and cancelling tickets. Every call needs the bearer token, which the
 * auth interceptor attaches automatically.
 */
@Injectable({ providedIn: 'root' })
export class TicketService {
  private readonly http = inject(HttpClient);

  book(request: BookTicketRequest): Observable<Ticket> {
    return this.http.post<Ticket>('/ticket', request);
  }

  getMine(): Observable<Ticket[]> {
    return this.http.get<Ticket[]>('/ticket');
  }

  /** Reschedules a ticket to a new date/time and returns the updated ticket. */
  update(ticketId: number, request: UpdateTicketRequest): Observable<Ticket> {
    return this.http.put<Ticket>(`/ticket/${ticketId}`, request);
  }

  cancel(ticketId: number): Observable<void> {
    return this.http.delete<void>(`/ticket/${ticketId}`);
  }
}
