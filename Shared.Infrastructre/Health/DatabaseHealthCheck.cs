using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Shared.Infrastructre.Context;

namespace Shared.Infrastructre.Health
{
    public sealed class DatabaseHealthCheck(AppDbContext dbContext) : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var x = dbContext.Database.ExecuteSqlRaw("Select 1");
                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(exception: ex);
            }
        }
    }
}
