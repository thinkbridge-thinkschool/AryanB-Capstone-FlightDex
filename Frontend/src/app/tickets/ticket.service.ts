import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BookTicketRequest, Ticket } from './ticket.models';

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

  cancel(ticketId: number): Observable<void> {
    return this.http.delete<void>(`/ticket/${ticketId}`);
  }
}
