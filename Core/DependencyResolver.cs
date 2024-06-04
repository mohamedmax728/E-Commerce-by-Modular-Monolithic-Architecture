using EasyRepository.EFCore.Generic;

namespace Core
{
    public static class DependencyResolver
    {
        public static void RegisterServices(
          IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddMetrics();
            services.AddMemoryCache();

            Modules.CustomerManagement.Api.DependencyInjection.RegisterServices(services);
            Modules.ProductCatalog.Api.DependencyInjection.RegisterServices(services);
            Modules.ShoppingCart.Api.DependencyInjection.RegisterServices(services);
            Modules.OrderManagement.Api.DependencyInjection.RegisterServices(services);
        }
    }
}
