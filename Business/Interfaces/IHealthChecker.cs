using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Business
{
    public interface IHealthChecker
    {
        Task<HealthReport> CheckHealthAsync();
    }
}