using SmartEventBooking.Application.Abstractions.Repositories;
using SmartEventBooking.Application.Abstractions.Services;
using SmartEventBooking.Application.DTOs.CreateEvent;
using SmartEventBooking.Application.DTOs.UpdateEvent;
using SmartEventBooking.Application.DTOs.Event;
using SmartEventBooking.Application.DTOs.Detail;
using SmartEventBooking.Application.DTOs.Venue;
using SmartEventBooking.Application.DTOs.Common;
using SmartEventBooking.Domain.Entities;

namespace SmartEventBooking.Application.Services
{
    public class EventService : IEventService
    {
        public readonly IEventRepository _repository;
        private readonly IVenueRepository _venueRepository;
        public EventService(IEventRepository repository, IVenueRepository venueRepository)
        {
            _repository = repository;
            _venueRepository = venueRepository;
        }

        public async Task<PaginatedListDto<EventDto>> GetAllAsync(int page = 1, int pageSize = 5, CancellationToken cancellationToken = default)
        {
            var skip = (page - 1) * pageSize;
            var events = await _repository.GetAllAsync(skip, pageSize, cancellationToken);
            var totalCount = await _repository.GetCountAsync(cancellationToken);

            var items = events.Select(e => new EventDto
            {
                Id = e.Id,
                Title = e.Title,
                Categories = e.EventCategories?.Select(ec => ec.Category.Name).ToList() ?? new List<string>(),
                StartDateTime = e.StartDateTime,
                EndDateTime = e.EndDateTime,
                Price = e.Price,
                Venue = e.Venue != null ? new VenueDto { Id = e.Venue.Id, Name = e.Venue.Name, Address = e.Venue.Location } : null
            }).ToList();

            return new PaginatedListDto<EventDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PaginatedListDto<EventDto>> GetUpcomingAsync(int page = 1, int pageSize = 5, EventSearchDto? searchDto = null, CancellationToken cancellationToken = default)
        {
            var skip = (page - 1) * pageSize;
            var events = await _repository.GetUpcomingAsync(skip, pageSize, searchDto, cancellationToken);
            var totalCount = await _repository.GetUpcomingCountAsync(searchDto, cancellationToken);

            var items = events.Select(e => new EventDto
            {
                Id = e.Id,
                Title = e.Title,
                Categories = e.EventCategories?.Select(ec => ec.Category.Name).ToList() ?? new List<string>(),
                StartDateTime = e.StartDateTime,
                EndDateTime = e.EndDateTime,
                Price = e.Price,
                Venue = e.Venue != null ? new VenueDto { Id = e.Venue.Id, Name = e.Venue.Name, Address = e.Venue.Location } : null
            }).ToList();

            return new PaginatedListDto<EventDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<DetailsDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var ev = await _repository.GetByIdAsync(id, cancellationToken);

            if (ev == null)
                return null;

            return new DetailsDto
            {
                Id = ev.Id,
                Title = ev.Title,
                Description = ev.Description,
                Banner = ev.Banner,                 
                StartDateTime = ev.StartDateTime,
                EndDateTime = ev.EndDateTime,
                TotalCapacity = ev.TotalCapacity,
                AvailableSeats = ev.AvailableSeats, 
                Price = ev.Price,
                Status = ev.Status,
                VenueId = ev.VenueId,
                Venue = ev.Venue != null ? new VenueDto { Id = ev.Venue.Id, Name = ev.Venue.Name, Address = ev.Venue.Location } : null,
                Categories = ev.EventCategories != null ? ev.EventCategories.Select(ec => ec.Category.Name).ToList() : new List<string>()
            };
        }

        public async Task<Guid?> CreateAsync(CreateEventDto request, CancellationToken cancellationToken = default)
        {
            var venueExists = await _venueRepository.ExistsAsync(request.VenueId, cancellationToken);

            if (!venueExists)
            {
                return null;
            }
                var newEvent = new Event(
                Guid.NewGuid(),
                request.Title,
                request.Description,
                request.StartDateTime,
                request.EndDateTime,
                request.TotalCapacity,
                request.Price,
                request.VenueId,
                request.Status.Value
            );

            if (!string.IsNullOrWhiteSpace(request.Banner))
            {
                newEvent.SetBanner(request.Banner);
            }

            await _repository.AddAsync(newEvent, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return newEvent.Id;
        }

        public async Task<bool> UpdateAsync(UpdateEventDto request, CancellationToken cancellationToken = default)
        {
            var existingEvent = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (existingEvent == null)
            {
               return false;
            }

            var venueExists = await _venueRepository.ExistsAsync(request.VenueId, cancellationToken);

            if (!venueExists)
            {
                return false;
            }

            existingEvent.Update(
                request.Title,
                request.Description,
                request.Banner,
                request.StartDateTime,
                request.EndDateTime,
                request.TotalCapacity,
                request.Price,
                request.VenueId,
                request.Status.Value
            );

            _repository.Update(existingEvent);

            await _repository.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<List<VenueDto>> GetVenuesAsync(CancellationToken cancellationToken = default)
        {
            var venues = await _venueRepository.GetAllAsync(cancellationToken);

            return venues.Select(v => new VenueDto
            {
                Id = v.Id,
                Name = v.Name
            }).ToList();
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var ev = await _repository.GetByIdAsync(id, cancellationToken);

            if (ev == null)
            {
                return false;
            }
                
            _repository.Delete(ev);
            await _repository.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}