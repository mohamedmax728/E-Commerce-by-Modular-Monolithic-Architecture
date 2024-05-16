using Modules.ShoppingCart.Application.Features;
using Modules.ShoppingCart.Application.Interfaces;
using Modules.ShoppingCart.Application.Mapping;

namespace Modules.ShoppingCart.Api
{
    public static class DependencyInjection
    {
        public static void RegisterServices(
          IServiceCollection services)
        {

            services.AddAutoMapper(typeof(ShoppingCartProfiler));
            services.AddScoped<IShoppingCartService, ShoppingCartService>();
        }
    }
}
