import { Component, inject, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { EventService } from '../../core/services/event.service';
import { Event } from '../../core/models/event.model';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-events-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './events-list.component.html',
  styleUrls: ['./events-list.component.scss']
})
export class EventsListComponent implements OnInit, OnDestroy {
  private eventService = inject(EventService);
  private cdr = inject(ChangeDetectorRef);
  private subscription?: Subscription;

  paginatedEvents: Event[] = [];
  isLoading = true;

  currentPage = 1;
  pageSize = 5;
  totalPages = 0;
  totalCount = 0;

  ngOnInit(): void {
    this.loadEvents();
  }


  loadEvents(): void {
    this.isLoading = true;
    this.subscription?.unsubscribe();
    
    this.subscription = this.eventService.getUpcomingEvents(this.currentPage, this.pageSize).subscribe({
      next: (response) => {
        this.paginatedEvents = response.items || [];
        this.totalCount = response.totalCount || 0;
        this.totalPages = response.totalPages || Math.ceil(this.totalCount / this.pageSize);
        
        this.isLoading = false;
        this.cdr.detectChanges();
        console.log('Data loaded successfully. isLoading is now:', this.isLoading);
      },
      error: (err) => {
        console.error('Error loading events:', err);
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.loadEvents();
    }
  }

  prevPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.loadEvents();
    }
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadEvents();
    }
  }

  getPages(): number[] {
    return Array.from({ length: this.totalPages }, (_, i) => i + 1);
  }
}
