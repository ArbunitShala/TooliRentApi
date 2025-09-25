using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.DTOs.Catalog;
using TooliRent.Core.Models.Catalog;

namespace TooliRent.Services.Services.Catalog.Mapping
{
    public class ToolProfile : Profile
    {
        public ToolProfile()
        {
            CreateMap<Tool, ToolListItemDto>()
                .ForMember(d => d.CategoryName, m => m.MapFrom(s => s.Category!.Name));

            CreateMap<Tool, ToolDetailsDto>()
                .ForMember(d => d.CategoryName, m => m.MapFrom(s => s.Category!.Name));
        }
    }
}
