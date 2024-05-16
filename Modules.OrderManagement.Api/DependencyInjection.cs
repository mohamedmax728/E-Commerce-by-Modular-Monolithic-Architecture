using Modules.OrderManagement.Application.Features;
using Modules.OrderManagement.Application.Interfaces;
using Modules.OrderManagement.Application.Mapping;

namespace Modules.OrderManagement.Api
{
    public static class DependencyInjection
    {
        public static void RegisterServices(
          IServiceCollection services)
        {
            services.AddAutoMapper(typeof(OrderProfiler));
            services.AddScoped<IOrderService, OrderService>();
        }
    }
}
