using System.Linq;
using System.Threading.Tasks;
using Pulumi.Azure.Sql;
using Xunit;

namespace Library.Infrastructure.Tests.Tests
{
    public class SqlDatabaseTests
    {
        private readonly MockStackHelper _stackHelper;
        
        public SqlDatabaseTests()
        {
            _stackHelper = new MockStackHelper();
        }
        
        [Fact]
        public async Task ShouldSetResourceGroupName()
        {
            var resources = await _stackHelper.TestAsync();

            var database = resources.OfType<Database>().FirstOrDefault();
            Assert.NotNull(database);

            var resourceGroupName = await database.ResourceGroupName.GetValueAsync();
            Assert.NotNull(resourceGroupName);
            Assert.Equal("library-rg", resourceGroupName);
        }
        
        [Fact]
        public async Task ShouldSetLocationFromResourceGroup()
        {
            var resources = await _stackHelper.TestAsync();

            var database = resources.OfType<Database>().FirstOrDefault();
            Assert.NotNull(database);

            var location = await database.Location.GetValueAsync();
            Assert.NotNull(location);
            Assert.Equal("CentralUS", location);
        }
        
        [Fact]
        public async Task ShouldBeNamedLocation()
        {
            var resources = await _stackHelper.TestAsync();

            var database = resources.OfType<Database>().FirstOrDefault();
            Assert.NotNull(database);

            var name = await database.Name.GetValueAsync();
            Assert.NotNull(name);
            Assert.Equal("Library", name);
        }
        
        [Fact]
        public async Task ShouldLinkToSqlServerDatabase()
        {
            var resources = await _stackHelper.TestAsync();

            var sqlServer = resources.OfType<SqlServer>().FirstOrDefault();
            Assert.NotNull(sqlServer);

            var serverName = await sqlServer.Name.GetValueAsync();
            
            var database = resources.OfType<Database>().FirstOrDefault();
            Assert.NotNull(database);

            var sqlServerName = await database.ServerName.GetValueAsync();
            Assert.NotNull(sqlServerName);
            Assert.Equal(serverName, sqlServerName);
        }
        
        [Fact]
        public async Task ShouldUseConfiguredSize()
        {
            var resources = await _stackHelper.TestAsync();

            var database = resources.OfType<Database>().FirstOrDefault();
            Assert.NotNull(database);

            var size = await database.RequestedServiceObjectiveName.GetValueAsync();
            Assert.NotNull(size);
            Assert.Equal(_stackHelper.Config.DatabaseSize, size);
        }
        
        [Fact]
        public async Task ShouldSetEnvironmentTag()
        {
            var resources = await _stackHelper.TestAsync();

            var database = resources.OfType<Database>().FirstOrDefault();
            Assert.NotNull(database);
            
            var tags = await database.Tags.GetValueAsync();
            Assert.NotNull(tags);
            Assert.True(tags.ContainsKey("environment"));
            Assert.Equal("dev", tags["environment"]);
        }
    }
}