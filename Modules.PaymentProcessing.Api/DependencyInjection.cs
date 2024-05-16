using Modules.PaymentProcessing.Application.Features;
using Modules.PaymentProcessing.Application.Interfaces;
using Modules.PaymentProcessing.Application.Mappings;
using Modules.PaymentProcessing.Domain.Helpers;

namespace Modules.PaymentProcessing.Api
{
    public static class DependencyInjection
    {
        public static void RegisterServices(
            WebApplicationBuilder webApplicationBuilder)
        {
            var _configuration = webApplicationBuilder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            // Add services to the container.
            var x = _configuration.GetSection("StripeSettings");
            webApplicationBuilder.Services.Configure<StripeSettings>(_configuration.GetSection("StripeSettings"));
            webApplicationBuilder.Services.AddAutoMapper(typeof(PaymentProfiller));
            webApplicationBuilder.Services.AddScoped<IPaymentService, PaymentService>();
        }
    }
}
