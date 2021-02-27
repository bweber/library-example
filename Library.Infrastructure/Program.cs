using System.Threading.Tasks;
using Pulumi;

namespace Library.Infrastructure
{
    internal static class Program
    {
        private static Task<int> Main() => Deployment.RunAsync<APIStack>();
    }
}