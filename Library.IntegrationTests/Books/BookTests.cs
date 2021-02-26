using System.Threading.Tasks;
using Library.IntegrationTests.Utilities;
using Xunit;

namespace Library.IntegrationTests.Books
{
    public class BookTests
    {
        [Fact]
        public async Task ShouldPerformBookLifecycle()
        {
            await VersionHelper.WaitForVersion();
        }
    }
}