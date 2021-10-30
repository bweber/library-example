using Microsoft.ApplicationInsights.DataContracts;

namespace Library.Common.Logging
{
    public static class TelemetryExtensions
    {
        public static void AddPropertyIfAbsent(this ISupportProperties supportProperties, string key, string value)
        {
            if (supportProperties.Properties.ContainsKey(key) || string.IsNullOrEmpty(value)) return;

            supportProperties.Properties.Add(key, value);
        }
    }
}
