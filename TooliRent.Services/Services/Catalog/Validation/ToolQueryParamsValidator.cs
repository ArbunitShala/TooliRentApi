using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.DTOs.Catalog;

namespace TooliRent.Services.Services.Catalog.Validation
{
    public class ToolQueryParamsValidator : AbstractValidator<ToolQueryParams>
    {
        private static readonly HashSet<string> AllowedSort = new(StringComparer.OrdinalIgnoreCase)
    { "name", "status", "category" };

        public ToolQueryParamsValidator()
        {
            RuleFor(x => x.SortBy)
                .Must(s => s == null || AllowedSort.Contains(s!))
                .WithMessage($"SortBy måste vara en av: {string.Join(", ", AllowedSort)}");
        }
    }
}
