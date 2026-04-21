import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Event } from '../models/event.model';
import { PaginatedList } from '../models/paginated-list.model';
import { EventSearchDto } from '../models/event-search.model';

@Injectable({
  providedIn: 'root'
})
export class EventService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7134/api/events';

  getUpcomingEvents(page: number = 1, pageSize: number = 5, search?: EventSearchDto): Observable<PaginatedList<Event>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (search) {
      if (search.keyword) params = params.set('keyword', search.keyword);
      if (search.categoryId) params = params.set('categoryId', search.categoryId.toString());
      if (search.startDate) params = params.set('startDate', search.startDate);
      if (search.endDate) params = params.set('endDate', search.endDate);
      if (search.location) params = params.set('location', search.location);
    }

    return this.http.get<PaginatedList<Event>>(`${this.apiUrl}/upcoming`, { params });
  }


  getEventById(id: string): Observable<Event> {
    return this.http.get<Event>(`${this.apiUrl}/${id}`);
  }
}
