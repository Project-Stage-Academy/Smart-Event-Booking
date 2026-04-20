using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Mvc;
using SmartEventBooking.Web.Services;
using Microsoft.AspNetCore.Authorization;
using SmartEventBooking.Application.DTOs.CreateEvent;
using SmartEventBooking.Application.DTOs.UpdateEvent;
using SmartEventBooking.Application.DTOs.Event;

namespace SmartEventBooking.Web.Controllers
{
   
    [Authorize(Roles = "Admin")]
    public class EventsController : Controller
    {
        public readonly EventService _service;

        public EventsController(EventService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var events = await _service.GetAllAsync();

            if (events == null)
            {
                return Content("events is NULL");
            } 

            return View(events ?? new List<EventDto>());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Venues = await _service.GetVenuesAsync();
            return View("~/Views/Events/Create.cshtml", new CreateEventDto());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEventDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Venues = await _service.GetVenuesAsync();
                return View("~/Views/Events/Create.cshtml", dto);
            }
            

            var id = await _service.CreateAsync(dto);

            return RedirectToAction("Details", new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var eventDto = await _service.GetByIdAsync(id);

            if (eventDto == null)
            {
                return NotFound();
            } 

            return View(eventDto); 
        }


        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var eventDto = await _service.GetByIdAsync(id);

            if (eventDto == null)
            {
                return NotFound();
            }
                

            ViewBag.Venues = await _service.GetVenuesAsync();

            var model = new UpdateEventDto
            {
                Id = eventDto.Id,
                Title = eventDto.Title,
                Description = eventDto.Description,
                StartDateTime = eventDto.StartDateTime,
                EndDateTime = eventDto.EndDateTime,
                TotalCapacity = eventDto.TotalCapacity,
                Price = eventDto.Price,
                VenueId = eventDto.VenueId,
                Banner = eventDto.Banner,
                Status = eventDto.Status
            };

            return View("~/Views/Events/Edit.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateEventDto dto)
        {
            
            if (!ModelState.IsValid)
            {
                ViewBag.Venues = await _service.GetVenuesAsync();
                return View("~/Views/Events/Edit.cshtml", dto);
            }

            var updated = await _service.UpdateAsync(dto);

            if (!updated)
            {
                return NotFound();
            }

            return RedirectToAction("Details", new { id = dto.Id });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);

            if(!deleted)
            {
                return NotFound();
            }

            TempData["Success"] = "Подію успішно видалено";
            return RedirectToAction("Index");
        }


    }
}
