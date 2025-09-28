using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.Models.Catalog;

namespace TooliRent.Core.DTOs.Catalog
{
    // Detaljerad vy för en enskild Tool
    public class ToolDetailsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public ToolStatus Status { get; set; }

        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = default!;
    }
}
