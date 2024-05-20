using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Modules.PaymentProcessing.Domain.Helpers;
using Stripe;
using Stripe.Checkout;
namespace Shared.Infrastructre.Health
{
    public class StripeHealthCheck(IOptions<StripeSettings> stripeSettings) : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            StripeConfiguration.ApiKey = stripeSettings.Value.SecretKey;

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> {
                    "card"
                },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "EGP",
                            UnitAmount = (long?)100,
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name  = "Jaket",
                                Description = "Jaket"
                            }
                        },
                        Quantity = 12313
                    }
                },
                Mode = "payment",
                SuccessUrl = "https://www.google.com/search?q=how+to+solve+problem+of+circle+reference+in+.net+core&rlz=1C1GCEU_enEG1067EG1067&oq=how+to+solve+problem+of+circle+reference+in+.net+core+&gs_lcrp=EgZjaHJvbWUyCwgAEEUYChg5GKABMgkIARAhGAoYoAHSAQg1MjgyajBqMagCALACAA&sourceid=chrome&ie=UTF-8"
            };
            try
            {
                var s = new SessionService().Create(options);

                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(exception: ex);
            }
        }
    }
}
