// ReSharper disable ObjectCreationAsStatement
#nullable enable
using Pulumi;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Web.V20210201;
using Pulumi.AzureNative.Web.V20210201.Inputs;
using Deployment = Pulumi.Deployment;
using ResourceArgs = Pulumi.ResourceArgs;

namespace Library.Infrastructure.Modules
{
    public class AppServicePlanModule : ComponentResource
    {
        [Output("id")]
        public Output<string> Id { get; set; }

        public AppServicePlanModule(string name, AppServicePlanModuleArgs args,
            ComponentResourceOptions? options = null, bool remote = false) :
            base("library:components:AppServicePlanModule", name, args, options, remote)
        {
            var appServicePlan = new AppServicePlan(name, new AppServicePlanArgs
            {
                Name = name,
                Location = args.ResourceGroupLocation,
                ResourceGroupName = args.ResourceGroupName,
                Kind = "Linux",
                Reserved = true,
                ZoneRedundant = true,
                Sku = new SkuDescriptionArgs
                {
                    Tier = args.AppServicePlanTier,
                    Size = args.AppServicePlanSize,
                    Capacity = args.AppServiceCapacity
                },
                Tags =
                {
                    { "environment", Deployment.Instance.StackName }
                }
            });

            new DiagnosticSettingsModule($"{name}-diag", new DiagnosticSettingsModuleArgs
            {
                ResourceId = appServicePlan.Id,
                LogAnalyticsWorkspaceId = args.LogAnalyticsWorkspaceId,
                MetricsCategories = new InputList<string>
                {
                    "AllMetrics"
                }
            });

            Id = appServicePlan.Id;
        }
    }

    public sealed class AppServicePlanModuleArgs : ResourceArgs
    {
        [Input("appServicePlanTier")]
        public Input<string>? AppServicePlanTier { get; init; }

        [Input("appServicePlanSize")]
        public Input<string>? AppServicePlanSize { get; init; }

        [Input("appServiceCapacity")]
        public Input<int>? AppServiceCapacity { get; init; } = null;

        [Input("logAnalyticsWorkspaceId")]
        public Input<string> LogAnalyticsWorkspaceId { get; init; } = null!;

        [Input("resourceGroupName")]
        public Input<string> ResourceGroupName { get; }

        [Input("resourceGroupLocation")]
        public Input<string> ResourceGroupLocation { get; }

        public AppServicePlanModuleArgs(ResourceGroup resourceGroup)
        {
            ResourceGroupName = resourceGroup.Name;
            ResourceGroupLocation = resourceGroup.Location;
        }
    }
}
