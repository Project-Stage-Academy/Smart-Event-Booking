import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { EventService } from '../../../core/services/event.service';
import { Event, EventStatus } from '../../../core/models/event.model';

@Component({
  selector: 'app-event-details',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './event-details.component.html',
  styleUrls: ['./event-details.component.scss']
})
export class EventDetailsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private eventService = inject(EventService);

  event = signal<Event | undefined>(undefined);
  isLoading = signal(true);
  error = signal<string | null>(null);

  ngOnInit(): void {
    const eventId = this.route.snapshot.paramMap.get('id');
    if (eventId) {
      this.loadEvent(eventId);
    } else {
      this.error.set('No event ID provided');
      this.isLoading.set(false);
    }
  }

  loadEvent(id: string): void {
    this.isLoading.set(true);
    this.error.set(null);
    this.eventService.getEventById(id).subscribe({
      next: (data) => {
        this.event.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.error.set('Failed to load event details. Please try again later.');
        this.isLoading.set(false);
        console.error('Error fetching event details', err);
      }
    });
  }

  onBookTicket(): void {
    // Note: Assuming there will be an auth service to check login later.
    // For now, if unauthenticated it might prompt for login, but since we don't have an auth service yet,
    // we'll just redirect to a hypothetical checkout or login page, or show an alert.
    console.log('Book ticket clicked for event:', this.event()?.id);
    alert('Booking functionality will be available soon!');
  }
}
