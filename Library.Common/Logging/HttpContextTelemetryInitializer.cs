using CorrelationId;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;

namespace Library.Common.Logging
{
    public class HttpContextTelemetryInitializer : ITelemetryInitializer
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextTelemetryInitializer(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Initialize(ITelemetry telemetry)
        {
            var telemetryProperties = telemetry as ISupportProperties;
            var context = _httpContextAccessor.HttpContext;

            if (telemetryProperties == null || context == null) return;

            if (context.Request.Headers.TryGetValue(CorrelationIdOptions.DefaultHeader, out var correlationId))
                telemetryProperties.AddPropertyIfAbsent("CorrelationId", correlationId);
        }
    }
}
