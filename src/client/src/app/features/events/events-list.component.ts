import { Component, inject, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { EventService } from '../../core/services/event.service';
import { CategoryService } from '../../core/services/category.service';
import { Event } from '../../core/models/event.model';
import { Category } from '../../core/models/category.model';
import { EventSearchDto } from '../../core/models/event-search.model';
import { Subscription } from 'rxjs';
import { getCategoryBg, getCategoryTextColor } from '../../shared/utils/category-color.util';

@Component({
  selector: 'app-events-list',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './events-list.component.html',
  styleUrls: ['./events-list.component.scss']
})
export class EventsListComponent implements OnInit, OnDestroy {
  private eventService = inject(EventService);
  private categoryService = inject(CategoryService);
  private cdr = inject(ChangeDetectorRef);
  private subscription?: Subscription;
  private categorySubscription?: Subscription;

  paginatedEvents: Event[] = [];
  isLoading = true;

  currentPage = 1;
  pageSize = 5;
  totalPages = 0;
  totalCount = 0;

  searchFilters: EventSearchDto = {};

  categories: Category[] = [];

  ngOnInit(): void {
    this.loadCategories();
    this.loadEvents();
  }

  loadCategories(): void {
    this.categorySubscription = this.categoryService.getCategories().subscribe({
      next: (response) => {
        this.categories = response;
        this.cdr.detectChanges(); 
      },
      error: (err) => console.error('Failed to load categories', err)
    });
  }

  applyFilters(): void {
    this.currentPage = 1;
    this.loadEvents();
  }

  clearFilters(): void {
    this.searchFilters = {};
    this.currentPage = 1;
    this.loadEvents();
  }

  loadEvents(): void {
    this.isLoading = true;
    this.subscription?.unsubscribe();
    
    this.subscription = this.eventService.getUpcomingEvents(this.currentPage, this.pageSize, this.searchFilters).subscribe({
      next: (response) => {
        this.paginatedEvents = response.items || [];
        this.totalCount = response.totalCount || 0;
        this.totalPages = response.totalPages || Math.ceil(this.totalCount / this.pageSize);
        
        this.isLoading = false;
        this.cdr.detectChanges(); 
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
    this.categorySubscription?.unsubscribe();
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

  getCategoryBg = getCategoryBg;
  getCategoryTextColor = getCategoryTextColor;
}