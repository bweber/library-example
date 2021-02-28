using Pulumi;
using Pulumi.Azure.Core;
using Pulumi.Azure.KeyVault;
using Pulumi.Azure.Sql;
using Pulumi.Random;

namespace Library.Infrastructure.Modules
{
    public class SqlDatabaseModule : ComponentResource
    {
        public SqlDatabaseModule(string name, SqlDatabaseModuleArgs args, ComponentResourceOptions options = null, bool remote = false) 
            : base("library:components:SqlDatabase", name, args, options, remote)
        {
            var adminUsername = new RandomString($"{name}-admin-username", new RandomStringArgs
            {
                Length = 16,
                Special = false,
                Number = false
            });

            var adminPassword = new RandomPassword($"{name}-admin-pass", new RandomPasswordArgs
            {
                Length = 32,
                Special = true
            });

            var sqlServer = new SqlServer(name, new SqlServerArgs
            {
                ResourceGroupName = args.ResourceGroupName,
                Location = args.ResourceGroupLocation,
                AdministratorLogin = adminUsername.Result,
                AdministratorLoginPassword = adminPassword.Result,
                Version = "12.0",
                Tags =
                {
                    { "environment", Deployment.Instance.StackName }
                }
            }, new CustomResourceOptions
            {
                Protect = true
            });

            var database = new Database($"{name}-db", new DatabaseArgs
            {
                Name = "Library",
                ResourceGroupName = args.ResourceGroupName,
                Location = args.ResourceGroupLocation,
                ServerName = sqlServer.Name,
                RequestedServiceObjectiveName = args.DatabaseSize,
                Tags =
                {
                    { "environment", Deployment.Instance.StackName }
                }
            }, new CustomResourceOptions
            {
                Protect = true
            });

            var connectionString =
                Output.Format($"Server=tcp:{sqlServer.Name}.database.windows.net;Database={database.Name};User Id={adminUsername.Result};Password={adminPassword.Result};");
            
            new Secret($"{name}-connection-string", new SecretArgs
            {
                Name = "ConnectionStrings--LibraryDB",
                KeyVaultId = args.KeyVaultId,
                Value = connectionString,
                Tags =
                {
                    { "environment", Deployment.Instance.StackName }
                }
            });

            // Allow app service access to the database: https://docs.microsoft.com/en-us/rest/api/sql/firewallrules/createorupdate
            new FirewallRule($"{name}-fw", new FirewallRuleArgs
            {
                ResourceGroupName = args.ResourceGroupName,
                ServerName = sqlServer.Name,
                StartIpAddress = "0.0.0.0",
                EndIpAddress = "0.0.0.0"
            });
        }
    }
    
    public sealed class SqlDatabaseModuleArgs : ResourceArgs 
    {
        [Input("databaseSize")]
        public Input<string> DatabaseSize { get; set; } = null!;
        
        [Input("keyVaultId")]
        public Input<string> KeyVaultId { get; set; } = null!;
        
        [Input("resourceGroupName")]
        public Input<string> ResourceGroupName { get; set; }
        
        [Input("resourceGroupLocation")]
        public Input<string> ResourceGroupLocation { get; set; }
        
        public SqlDatabaseModuleArgs(ResourceGroup resourceGroup)
        {
            ResourceGroupName = resourceGroup.Name;
            ResourceGroupLocation = resourceGroup.Location;
        }
    }
}