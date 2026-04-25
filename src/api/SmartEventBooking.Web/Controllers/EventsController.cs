using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using SmartEventBooking.Application.DTOs.CreateEvent;
using SmartEventBooking.Application.DTOs.Event;
using SmartEventBooking.Application.DTOs.UpdateEvent;
using SmartEventBooking.Application.Abstractions.Services;
using SmartEventBooking.Domain.Constants;

namespace SmartEventBooking.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEventService _service;
    private readonly IValidator<CreateEventDto> _createValidator;

    public EventsController(IEventService service, IValidator<CreateEventDto> createValidator)
    {
        _service = service;
        _createValidator = createValidator;
    }

    [HttpGet("upcoming")]
    [AllowAnonymous]
    public async Task<IActionResult> GetUpcoming([FromQuery] int page = 1, [FromQuery] int pageSize = 5, [FromQuery] EventSearchDto? searchDto = null)
    {
        var result = await _service.GetUpcomingAsync(page, pageSize, searchDto);
        return Ok(result);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
        var result = await _service.GetAllAsync(page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var eventDto = await _service.GetByIdAsync(id);
        if (eventDto == null)
            return NotFound();

        return Ok(eventDto);
    }

    [HttpPost]
    [Authorize(Roles = RoleConstants.Admin)]
    public async Task<IActionResult> Create(CreateEventDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var id = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = RoleConstants.Admin)]
    public async Task<IActionResult> Edit(Guid id, UpdateEventDto dto)
    {
        if (id != dto.Id)
            return BadRequest("Id mismatch.");

        var updated = await _service.UpdateAsync(dto);
        if (!updated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = RoleConstants.Admin)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
