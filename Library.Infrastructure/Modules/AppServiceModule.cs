// ReSharper disable ObjectCreationAsStatement
using Pulumi;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;
using Deployment = Pulumi.Deployment;
using ManagedServiceIdentityType = Pulumi.AzureNative.Web.ManagedServiceIdentityType;
using ResourceArgs = Pulumi.ResourceArgs;

namespace Library.Infrastructure.Modules
{
    public class AppServiceModule : ComponentResource
    {
        [Output("id")]
        public Output<string> Id { get; set; }

        [Output("name")]
        public Output<string> Name { get; set; }

        [Output("principalId")]
        public Output<string> PrincipalId { get; set; }

        public AppServiceModule(string name, AppServiceModuleArgs args, ComponentResourceOptions options = null,
            bool remote = false) : base("library:components:AppService", name, args, options, remote)
        {
            var appService = new WebApp(name, new WebAppArgs
            {
                Name = name,
                Location = args.ResourceGroupLocation,
                ResourceGroupName = args.ResourceGroupName,
                ServerFarmId = args.AppServicePlanId,
                HttpsOnly = true,
                Identity = new ManagedServiceIdentityArgs
                {
                    Type = ManagedServiceIdentityType.SystemAssigned
                },
                SiteConfig = new SiteConfigArgs
                {
                    LinuxFxVersion = "DOCKER|microsoft/azure-appservices-go-quickstart",
                    FtpsState = "Disabled",
                    HealthCheckPath = "/healthz",
                    RemoteDebuggingEnabled = false,
                    MinTlsVersion = "1.2"
                },
                Tags =
                {
                    { "environment", Deployment.Instance.StackName }
                }
            });

            new WebAppDiagnosticLogsConfiguration($"{name}-logs", new WebAppDiagnosticLogsConfigurationArgs
            {
                Name = $"{name}-logs",
                ResourceGroupName = args.ResourceGroupName,
                HttpLogs = new HttpLogsConfigArgs
                {
                    FileSystem = new FileSystemHttpLogsConfigArgs
                    {
                        RetentionInDays = 14,
                        RetentionInMb = 35
                    }
                }
            });

            new DiagnosticSettingsModule($"{name}-diag", new DiagnosticSettingsModuleArgs
            {
                ResourceId = appService.Id,
                LogAnalyticsWorkspaceId = args.LogAnalyticsWorkspaceId,
                LogCategories =
                {
                    "AppServiceAntivirusScanAuditLogs",
                    "AppServiceHTTPLogs",
                    "AppServiceConsoleLogs",
                    "AppServiceAppLogs",
                    "AppServiceFileAuditLogs",
                    "AppServiceAuditLogs",
                    "AppServiceIPSecAuditLogs",
                    "AppServicePlatformLogs"
                },
                MetricsCategories = new InputList<string>
                {
                    "AllMetrics"
                }
            });

            Id = appService.Id;
            Name = appService.Name;
            PrincipalId = appService.Identity.Apply(i => i.PrincipalId!);
        }
    }

    public sealed class AppServiceModuleArgs : ResourceArgs
    {
        [Input("appServicePlanId")]
        public Input<string> AppServicePlanId { get; init; } = null!;

        [Input("logAnalyticsWorkspaceId")]
        public Input<string> LogAnalyticsWorkspaceId { get; init; } = null!;

        [Input("resourceGroupName")]
        public Input<string> ResourceGroupName { get; }

        [Input("resourceGroupLocation")]
        public Input<string> ResourceGroupLocation { get; }

        public AppServiceModuleArgs(ResourceGroup resourceGroup)
        {
            ResourceGroupName = resourceGroup.Name;
            ResourceGroupLocation = resourceGroup.Location;
        }
    }
}
