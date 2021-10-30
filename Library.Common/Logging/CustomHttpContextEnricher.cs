using CorrelationId;
using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;

namespace Library.Common.Logging
{
    public class CustomHttpContextEnricher : ILogEventEnricher
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomHttpContextEnricher(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (_httpContextAccessor.HttpContext == null) return;

            if (_httpContextAccessor.HttpContext.Request.Headers.TryGetValue(CorrelationIdOptions.DefaultHeader,
                out var correlationId))
            {
                var correlationIdProperty = new LogEventProperty("CorrelationId", new ScalarValue(correlationId));
                logEvent.AddPropertyIfAbsent(correlationIdProperty);
            }
        }
    }
}
