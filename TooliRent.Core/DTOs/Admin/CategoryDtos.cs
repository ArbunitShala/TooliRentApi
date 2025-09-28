using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Core.DTOs.Admin
{
    public record CategoryDto(Guid Id, string Name, string? Description);
    public record CreateCategoryRequest(string Name, string? Description);
    public record UpdateCategoryRequest(string Name, string? Description);
}
