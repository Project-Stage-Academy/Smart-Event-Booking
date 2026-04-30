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

        RuleForEach(x => x.Categories).ChildRules(category =>
        {
            category.RuleFor(c => c)
                .Must(c => c.Id.HasValue || !string.IsNullOrWhiteSpace(c.Name))
                .WithMessage("Each category must specify either an existing Id or a new Name.");

            category.RuleFor(c => c)
                .Must(c => !(c.Id.HasValue && !string.IsNullOrWhiteSpace(c.Name)))
                .WithMessage("Each category must specify either an Id or a Name, not both.");
        });
    }
}
