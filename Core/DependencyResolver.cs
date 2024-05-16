using EasyRepository.EFCore.Generic;

namespace Core
{
    public static class DependencyResolver
    {
        public static void RegisterServices(
          IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            //services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
