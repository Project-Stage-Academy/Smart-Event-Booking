using FluentValidation;
using SmartEventBooking.Application.DTOs.CreateEvent;

namespace SmartEventBooking.Application.Validators;

public class CreateEventDtoValidator : AbstractValidator<CreateEventDto>
{
    public CreateEventDtoValidator()
    {
        RuleFor(x => x.StartDateTime)
            .GreaterThanOrEqualTo(x => DateTime.UtcNow)
            .WithMessage("Start date cannot be in the past.");

        RuleFor(x => x.EndDateTime)
            .GreaterThan(x => x.StartDateTime)
            .WithMessage("End date must be after start date.");
    }
}
