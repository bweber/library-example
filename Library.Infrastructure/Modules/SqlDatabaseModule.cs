// ReSharper disable ObjectCreationAsStatement
using Pulumi;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Sql;
using Pulumi.AzureNative.Sql.Inputs;
using Pulumi.Random;
using Deployment = Pulumi.Deployment;
using ResourceArgs = Pulumi.ResourceArgs;

namespace Library.Infrastructure.Modules
{
    public class SqlDatabaseModule : ComponentResource
    {
        [Output("connectionString")]
        public Output<string> ConnectionString { get; set; }

        public SqlDatabaseModule(string name, SqlDatabaseModuleArgs args, ComponentResourceOptions options = null,
            bool remote = false)  : base("library:components:SqlDatabaseModule", name, args, options, remote)
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

            var sqlServer = new Server(name, new ServerArgs
            {
                ServerName = name,
                ResourceGroupName = args.ResourceGroupName,
                Location = args.ResourceGroupLocation,
                AdministratorLogin = adminUsername.Result,
                AdministratorLoginPassword = adminPassword.Result,
                PublicNetworkAccess = ServerPublicNetworkAccess.Enabled,
                Version = "12.0",
                MinimalTlsVersion = "1.2",
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
                DatabaseName = "Library",
                ResourceGroupName = args.ResourceGroupName,
                Location = args.ResourceGroupLocation,
                ServerName = sqlServer.Name,
                Sku = new SkuArgs
                {
                    Name = args.DatabaseSize
                },
                Tags =
                {
                    { "environment", Deployment.Instance.StackName }
                }
            }, new CustomResourceOptions
            {
                Protect = true
            });

            new DiagnosticSettingsModule($"{name}-server-diag", new DiagnosticSettingsModuleArgs
            {
                ResourceId = sqlServer.Id,
                LogAnalyticsWorkspaceId = args.LogAnalyticsWorkspaceId,
                MetricsCategories = new InputList<string>
                {
                    "AllMetrics"
                }
            });

            new DiagnosticSettingsModule($"{name}-db-diag", new DiagnosticSettingsModuleArgs
            {
                ResourceId = database.Id,
                LogAnalyticsWorkspaceId = args.LogAnalyticsWorkspaceId,
                LogCategories =
                {
                    "SQLInsights",
                    "AutomaticTuning",
                    "QueryStoreRuntimeStatistics",
                    "QueryStoreWaitStatistics",
                    "Errors",
                    "DatabaseWaitStatistics",
                    "Timeouts",
                    "Blocks",
                    "Deadlocks",
                    "DevOpsOperationsAudit",
                    "SQLSecurityAuditEvents"
                },
                MetricsCategories =
                {
                    "Basic",
                    "InstanceAndAppAdvanced",
                    "WorkloadManagement"
                }
            });

            new ExtendedServerBlobAuditingPolicy($"{name}-server-audit-policy", new ExtendedServerBlobAuditingPolicyArgs
            {
                ResourceGroupName = args.ResourceGroupName,
                ServerName = sqlServer.Name,
                State = BlobAuditingPolicyState.Enabled,
                IsAzureMonitorTargetEnabled = true
            });

            new ExtendedDatabaseBlobAuditingPolicy($"{name}-database-audit-policy", new ExtendedDatabaseBlobAuditingPolicyArgs
            {
                ResourceGroupName = args.ResourceGroupName,
                DatabaseName = database.Name,
                ServerName = sqlServer.Name,
                State = BlobAuditingPolicyState.Enabled,
                IsAzureMonitorTargetEnabled = true
            });

            ConnectionString =
                Output.Format($"Server=tcp:{sqlServer.Name}.database.windows.net;Database={database.Name};User Id={adminUsername.Result};Password={adminPassword.Result};");
        }
    }

    public sealed class SqlDatabaseModuleArgs : ResourceArgs
    {
        [Input("databaseSize")]
        public Input<string> DatabaseSize { get; init; } = null!;

        [Input("logAnalyticsWorkspaceId")]
        public Input<string> LogAnalyticsWorkspaceId { get; init; } = null!;

        [Input("resourceGroupName")]
        public Input<string> ResourceGroupName { get; }

        [Input("resourceGroupLocation")]
        public Input<string> ResourceGroupLocation { get; }

        public SqlDatabaseModuleArgs(ResourceGroup resourceGroup)
        {
            ResourceGroupName = resourceGroup.Name;
            ResourceGroupLocation = resourceGroup.Location;
        }
    }
}
