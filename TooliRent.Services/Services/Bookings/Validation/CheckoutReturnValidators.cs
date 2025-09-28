using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.DTOs.Bookings;

namespace TooliRent.Services.Services.Bookings.Validation
{
    public class CheckoutRequestValidator : AbstractValidator<CheckoutRequest>
    {
        public CheckoutRequestValidator()
        {
            // WhenUtc är valfri, men om den skickas får den inte vara slagskadat värde
            RuleFor(x => x.WhenUtc).Must(_ => true); // plats för utökning
        }
    }

    public class ReturnRequestValidator : AbstractValidator<ReturnRequest>
    {
        public ReturnRequestValidator()
        {
            RuleFor(x => x.WhenUtc).Must(_ => true);
        }
    }
}
