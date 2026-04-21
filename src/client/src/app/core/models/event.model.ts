export interface Event {
  id: string;
  title: string;
  description?: string;
  categories?: string[];
  startDateTime: Date | string;
  endDateTime: Date | string;
  totalCapacity: number;
  availableSeats: number;
  price: number;
  status: EventStatus;
  venueId: string;
  venue?: Venue;
  banner?: string;
}

export enum EventStatus {
  // Add status values according to your Domain.Enums.EventStatus
  Draft = 0,
  Published = 1,
  Cancelled = 2
}

export interface Venue {
  id: string;
  name: string;
  address?: string;
}
