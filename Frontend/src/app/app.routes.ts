import { Routes } from '@angular/router';
import { authGuard } from './auth/auth.guard';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'timetable' },
  {
    path: 'timetable',
    title: 'Timetable · FlightDex',
    loadComponent: () => import('./timetable/timetable').then(m => m.Timetable),
  },
  {
    path: 'login',
    title: 'Sign in · FlightDex',
    loadComponent: () => import('./auth/login').then(m => m.Login),
  },
  {
    path: 'book',
    title: 'Book Tickets · FlightDex',
    canActivate: [authGuard],
    loadComponent: () => import('./book/book-tickets').then(m => m.BookTickets),
  },
  {
    path: 'mytickets',
    title: 'My Tickets · FlightDex',
    canActivate: [authGuard],
    loadComponent: () => import('./tickets/my-tickets').then(m => m.MyTickets),
  },
  { path: '**', redirectTo: 'timetable' },
];
