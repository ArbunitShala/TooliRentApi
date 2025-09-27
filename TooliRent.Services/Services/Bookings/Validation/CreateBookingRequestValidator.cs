using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.DTOs.Bookings;

namespace TooliRent.Services.Services.Bookings.Validation
{
    public class CreateBookingRequestValidator : AbstractValidator<CreateBookingRequest>
    {
        public CreateBookingRequestValidator()
        {
            RuleFor(x => x.StartUtc)
                .LessThan(x => x.EndUtc).WithMessage("Start måste vara före End.")
                .GreaterThan(DateTime.UtcNow.AddMinutes(-1)).WithMessage("Start kan inte ligga i dåtiden.");

            RuleFor(x => x.EndUtc)
                .GreaterThan(DateTime.UtcNow).WithMessage("End måste ligga i framtiden.");

            RuleFor(x => x.ToolIds)
                .NotEmpty().WithMessage("Minst ett verktyg måste väljas.")
                .Must(list => list.Distinct().Count() == list.Count).WithMessage("Dubbelverktyg i listan.")
                .Must(list => list.Count <= 20).WithMessage("Max 20 verktyg per bokning.");
        }
    }
}
