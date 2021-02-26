using Microsoft.Extensions.Hosting;

namespace Library.Common.Extensions
{
    public static class HostEnvironmentExtensions
    {
        public static bool IsAcceptance(this IHostEnvironment environment)
        {
            return environment.IsEnvironment("Acceptance");
        }
    }
}