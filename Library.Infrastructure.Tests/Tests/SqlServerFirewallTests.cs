using System.Linq;
using System.Threading.Tasks;
using Pulumi.Azure.Sql;
using Xunit;

namespace Library.Infrastructure.Tests.Tests
{
    public class SqlServerFirewallTests
    {
        private readonly MockStackHelper _stackHelper;
        
        public SqlServerFirewallTests()
        {
            _stackHelper = new MockStackHelper();
        }
        
        [Fact]
        public async Task ShouldSetResourceGroupName()
        {
            var resources = await _stackHelper.TestAsync();

            var firewallRule = resources.OfType<FirewallRule>().FirstOrDefault();
            Assert.NotNull(firewallRule);

            var resourceGroupName = await firewallRule.ResourceGroupName.GetValueAsync();
            Assert.NotNull(resourceGroupName);
            Assert.Equal("library-rg", resourceGroupName);
        }
        
        [Fact]
        public async Task ShouldLinkToSqlServerDatabase()
        {
            var resources = await _stackHelper.TestAsync();

            var sqlServer = resources.OfType<SqlServer>().FirstOrDefault();
            Assert.NotNull(sqlServer);

            var serverName = await sqlServer.Name.GetValueAsync();
            
            var firewallRule = resources.OfType<FirewallRule>().FirstOrDefault();
            Assert.NotNull(firewallRule);

            var sqlServerName = await firewallRule.ServerName.GetValueAsync();
            Assert.NotNull(sqlServerName);
            Assert.Equal(serverName, sqlServerName);
        }
        
        [Fact]
        public async Task ShouldHaveLocalhostStartIp()
        {
            var resources = await _stackHelper.TestAsync();

            var firewallRule = resources.OfType<FirewallRule>().FirstOrDefault();
            Assert.NotNull(firewallRule);

            var ipAddress = await firewallRule.StartIpAddress.GetValueAsync();
            Assert.NotNull(ipAddress);
            Assert.Equal("0.0.0.0", ipAddress);
        }
        
        [Fact]
        public async Task ShouldHaveLocalhostEndIp()
        {
            var resources = await _stackHelper.TestAsync();

            var firewallRule = resources.OfType<FirewallRule>().FirstOrDefault();
            Assert.NotNull(firewallRule);

            var ipAddress = await firewallRule.EndIpAddress.GetValueAsync();
            Assert.NotNull(ipAddress);
            Assert.Equal("0.0.0.0", ipAddress);
        }
    }
}