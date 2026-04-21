import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Event } from '../models/event.model';
import { PaginatedList } from '../models/paginated-list.model';

@Injectable({
  providedIn: 'root'
})
export class EventService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7134/api/events';

  getUpcomingEvents(page: number = 1, pageSize: number = 5): Observable<PaginatedList<Event>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get<PaginatedList<Event>>(`${this.apiUrl}/upcoming`, { params });
  }


  getEventById(id: string): Observable<Event> {
    return this.http.get<Event>(`${this.apiUrl}/${id}`);
  }
}
