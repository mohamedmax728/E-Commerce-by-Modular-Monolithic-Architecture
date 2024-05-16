using Modules.CustomerManagement.Application.Features;
using Modules.CustomerManagement.Application.Interfaces;
using Modules.CustomerManagement.Application.Mappings;

namespace Modules.CustomerManagement.Api
{
    public static class DependencyInjection
    {
        public static void RegisterServices(
          IServiceCollection services)
        {
            services.AddAutoMapper(typeof(UserProfile));
            services.AddScoped<IAuthService, AuthService>();
        }
    }
}
