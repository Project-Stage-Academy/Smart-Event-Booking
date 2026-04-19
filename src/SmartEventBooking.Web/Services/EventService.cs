using SmartEventBooking.Application.Abstractions.Repositories;
using SmartEventBooking.Application.DTOs.CreateEvent;
using SmartEventBooking.Application.DTOs.UpdateEvent;
using SmartEventBooking.Application.DTOs.Event;
using SmartEventBooking.Application.DTOs.Detail;
using SmartEventBooking.Application.DTOs.Venue;
using SmartEventBooking.Domain.Entities;


namespace SmartEventBooking.Web.Services
{
    public class EventService
    {
        public readonly IEventRepository _repository;
        private readonly IVenueRepository _venueRepository;
        public EventService(IEventRepository repository, IVenueRepository venueRepository)
        {
            _repository = repository;
            _venueRepository = venueRepository;
        }

        public async Task<List<EventDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var events = await _repository.GetAllAsync(0, 100, cancellationToken);

            return events.Select(e => new EventDto
            {
                Id = e.Id,
                Title = e.Title,
                StartDateTime = e.StartDateTime,
                EndDateTime = e.EndDateTime,
                Price = e.Price
            }).ToList();
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
                VenueId = ev.VenueId
            };
        }

        public async Task<Guid> CreateAsync(CreateEventDto request, CancellationToken cancellationToken = default)
        {
            var newEvent = new Event(
                Guid.NewGuid(),
                request.Title,
                request.Description,
                request.StartDateTime,
                request.EndDateTime,
                request.TotalCapacity,
                request.Price,
                request.VenueId,
                request.Status
            );

            if (!string.IsNullOrWhiteSpace(request.Banner))
            {
                newEvent.SetBanner(request.Banner);
            }

            await _repository.AddAsync(newEvent, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return newEvent.Id;
        }

        public async Task UpdateAsync(UpdateEventDto request, CancellationToken cancellationToken = default)
        {
            var existingEvent = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (existingEvent == null)
                throw new Exception("Event not found");

            existingEvent.Update(
                request.Title,
                request.Description,
                request.Banner,
                request.StartDateTime,
                request.EndDateTime,
                request.TotalCapacity,
                request.Price,
                request.VenueId,
                request.Status
            );

            _repository.Update(existingEvent);

            await _repository.SaveChangesAsync(cancellationToken);
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

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var ev = await _repository.GetByIdAsync(id, cancellationToken);

            if (ev == null)
                throw new Exception("Event not found");

            _repository.Delete(ev);
            await _repository.SaveChangesAsync(cancellationToken);
        }

    }
}
