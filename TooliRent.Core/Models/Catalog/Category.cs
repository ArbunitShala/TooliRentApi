using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Core.Models.Catalog
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }

        public ICollection<Tool> Tools { get; set; } = new List<Tool>();
    }
}
