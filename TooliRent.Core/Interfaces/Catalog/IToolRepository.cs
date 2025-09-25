using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.Models.Catalog;

namespace TooliRent.Core.Interfaces.Catalog
{
    // Dataåtkomst för Tool (utan paging)
    public interface IToolRepository
    {
        Task<IReadOnlyList<Tool>> SearchAsync(
            string? search,
            Guid? categoryId,
            ToolStatus? status,
            bool? onlyAvailable,
            string? sortBy,
            bool sortDesc,
            CancellationToken ct = default);

        Task<Tool?> GetByIdAsync(Guid id, CancellationToken ct = default);
    }
}
