using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.Models.Catalog; // För ToolStatus

namespace TooliRent.Core.DTOs.Admin
{
    public record ToolAdminDto(
        Guid Id,
        string Name,
        string? Description,
        ToolStatus Status,
        Guid CategoryId,
        string CategoryName
    );

    public record CreateToolAdminRequest(
        string Name,
        string? Description,
        ToolStatus Status,
        Guid CategoryId
    );

    public record UpdateToolAdminRequest(
        string Name,
        string? Description,
        ToolStatus Status,
        Guid CategoryId
    );
}
