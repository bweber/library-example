using System;
using Pulumi;
using Pulumi.Azure.AppInsights;
using Pulumi.Azure.AppService;
using Pulumi.Azure.AppService.Inputs;
using Pulumi.Azure.Core;
using Pulumi.Azure.KeyVault;

// ReSharper disable ObjectCreationAsStatement

namespace Library.Infrastructure.Modules
{
    public class AppServiceModule : ComponentResource
    {
        [Output("name")]
        public Output<string> Name { get; private set; }
        
        [Output("defaultSiteHostname")]
        public Output<string> DefaultSiteHostname { get; private set; }
        
        public AppServiceModule(string name, AppServiceModuleArgs args, ComponentResourceOptions options = null, bool remote = false) 
            : base("library:components:AppService", name, args, options, remote)
        {
            var appInsights = new Insights($"{name}-insights", new InsightsArgs
            {
                Location = args.ResourceGroupLocation,
                ResourceGroupName = args.ResourceGroupName,
                ApplicationType = "web",
                Tags =
                {
                    { "environment", Deployment.Instance.StackName }
                }
            });
            
            var appService = new AppService(name, new AppServiceArgs
            {
                Location = args.ResourceGroupLocation,
                ResourceGroupName = args.ResourceGroupName,
                AppServicePlanId = args.AppServicePlanId,
                HttpsOnly = true,
                Identity = new AppServiceIdentityArgs
                {
                    Type = "SystemAssigned"
                },
                AppSettings =
                {
                    { "WEBSITES_ENABLE_APP_SERVICE_STORAGE", "false" },
                    { "WEBSITES_CONTAINER_START_TIME_LIMIT", "1800" },
                    { "ASPNETCORE_ENVIRONMENT", args.AspnetEnvironment },
                    { "APPINSIGHTS_INSTRUMENTATIONKEY", appInsights.InstrumentationKey },
                    { "KeyVaultName", args.KeyVaultName }
                },
                SiteConfig = new AppServiceSiteConfigArgs
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
                },
                Logs = new AppServiceLogsArgs
                {
                    HttpLogs = new AppServiceLogsHttpLogsArgs
                    {
                        FileSystem = new AppServiceLogsHttpLogsFileSystemArgs
                        {
                            RetentionInDays = 14,
                            RetentionInMb = 35
                        }
                    }
                }
            });
            
            var appServicePrincipalId = appService.Identity.Apply(id =>
                string.IsNullOrEmpty(id.PrincipalId) ? throw new ArgumentNullException() : id.PrincipalId);
            
            new AccessPolicy($"{name}-policy", new AccessPolicyArgs
            {
                KeyVaultId = args.KeyVaultId,
                TenantId = args.TenantId,
                ObjectId = appServicePrincipalId,
                SecretPermissions = { "get", "list" }
            });

            Name = appService.Name;
            DefaultSiteHostname = appService.DefaultSiteHostname;
        }
    }
    
    public sealed class AppServiceModuleArgs : ResourceArgs 
    {
        [Input("aspnetEnvironment")]
        public Input<string> AspnetEnvironment { get; set; } = null!;
        
        [Input("appServicePlanId")]
        public Input<string> AppServicePlanId { get; set; } = null!;
        
        [Input("tenantId")]
        public Input<string> TenantId { get; set; } = null!;
        
        [Input("keyVaultId")]
        public Input<string> KeyVaultId { get; set; } = null!;
        
        [Input("keyVaultName")]
        public Input<string> KeyVaultName { get; set; } = null!;
        
        [Input("resourceGroupName")]
        public Input<string> ResourceGroupName { get; set; }
        
        [Input("resourceGroupLocation")]
        public Input<string> ResourceGroupLocation { get; set; }
        
        public AppServiceModuleArgs(ResourceGroup resourceGroup)
        {
            ResourceGroupName = resourceGroup.Name;
            ResourceGroupLocation = resourceGroup.Location;
        }
    }
}