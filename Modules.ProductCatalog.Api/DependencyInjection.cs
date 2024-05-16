using Modules.ProductCatalog.Application.Features;
using Modules.ProductCatalog.Application.Interfaces;
using Modules.ProductCatalog.Application.Mapping;

namespace Modules.ProductCatalog.Api
{
    public static class DependencyInjection
    {
        public static void RegisterServices(
          IServiceCollection services)
        {
            services.AddAutoMapper(typeof(productProfiler));
            services.AddScoped<IProductService, ProductService>();
        }
    }
}
