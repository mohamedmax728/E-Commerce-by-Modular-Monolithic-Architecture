using AutoMapper;
using Modules.OrderManagement.Application.Dtos;
using Shared.Utilities.Models;
using Shared.Utilities.Models.Entities;

namespace Modules.OrderManagement.Application.Mapping
{
    public class OrderProfiler : Profile
    {
        public OrderProfiler()
        {
            CreateMap<Order, OrderDetailsDto>();
            CreateMap<Order, OrderListDto>();
            CreateMap<OrderUpdateDto, Order>();
            CreateMap<OrderCreateDto, Order>();
            CreateMap<AuditModel, AuditModelVM>();
        }
    }
}
