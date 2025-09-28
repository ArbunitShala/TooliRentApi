using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.DTOs.Bookings;
using TooliRent.Core.Models.Bookings;

namespace TooliRent.Services.Services.Bookings.Mapping
{
    public class BookingsProfile : Profile
    {
        public BookingsProfile()
        {
            CreateMap<Booking, BookingDetailsDto>()
                .ForMember(d => d.StartUtc, m => m.MapFrom(s => s.StartTime))
                .ForMember(d => d.EndUtc, m => m.MapFrom(s => s.EndTime))
                .ForMember(d => d.PickedUpAtUtc, m => m.MapFrom(s => s.PickedUpAtUtc))
                .ForMember(d => d.ReturnedAtUtc, m => m.MapFrom(s => s.ReturnedAtUtc))
                .ForMember(d => d.Status, m => m.MapFrom(s => s.Status))
                .ForMember(d => d.Tools, m => m.MapFrom(s =>
                    s.BookingTools.Select(bt =>
                        new BookingDetailsDto.ToolItem(
                        bt.ToolId,
                        bt.Tool != null ? bt.Tool.Name : string.Empty))));

            CreateMap<Booking, BookingSummaryDto>()
                .ForMember(d => d.StartUtc, m => m.MapFrom(s => s.StartTime))
                .ForMember(d => d.EndUtc, m => m.MapFrom(s => s.EndTime))
                .ForMember(d => d.Status, m => m.MapFrom(s => s.Status))
                .ForMember(d => d.ToolCount, m => m.MapFrom(s => s.BookingTools.Count));
        }
    }
}
