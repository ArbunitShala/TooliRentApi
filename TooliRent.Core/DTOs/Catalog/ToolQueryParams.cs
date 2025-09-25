using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.Models.Catalog;

namespace TooliRent.Core.DTOs.Catalog
{
    // Query-parametrar för listning/filter/sortering (ingen paging).
    public class ToolQueryParams
    {
        public string? Search { get; set; }
        public Guid? CategoryId { get; set; }
        public ToolStatus? Status { get; set; }

        //  true => returnera endast verktyg som är i Status = Available.
        public bool? OnlyAvailable { get; set; }

        // name | status | category (default: name)
        public string? SortBy { get; set; } = "name";

        // true = fallande ordning, false = stigande.
        public bool SortDesc { get; set; } = false;
    }
}
