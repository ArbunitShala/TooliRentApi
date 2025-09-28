using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.DTOs.Admin;
using TooliRent.Core.Models.Catalog;
using TooliRent.Infrastructure.Auth;

namespace TooliRent.Services.Services.Admin.Mapping
{
    public class AdminProfile : Profile
    {
        public AdminProfile()
        {
            // Category
            CreateMap<Category, CategoryDto>();
            CreateMap<CreateCategoryRequest, Category>();
            CreateMap<UpdateCategoryRequest, Category>();

            // Tool
            CreateMap<Tool, ToolAdminDto>()
                .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name));
            CreateMap<CreateToolAdminRequest, Tool>();
            CreateMap<UpdateToolAdminRequest, Tool>();

            // User (Role fylls i service via UserManager)
            CreateMap<ApplicationUser, UserAdminDto>()
                .ForMember(d => d.Role, o => o.Ignore());
        }
    }
}
