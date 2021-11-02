using Pulumi;
using Pulumi.AzureNative.Insights.V20200202Preview;
using Pulumi.AzureNative.Resources;
using Deployment = Pulumi.Deployment;
using ResourceArgs = Pulumi.ResourceArgs;

namespace Library.Infrastructure.Modules
{
    public class ApplicationInsightsModule : ComponentResource
    {
        [Output("Id")]
        public Output<string> Id { get; set; }

        [Output("instrumentationKey")]
        public Output<string> InstrumentationKey { get; set; }

        public ApplicationInsightsModule(string name, ApplicationInsightsModuleArgs args,
            ComponentResourceOptions options = null, bool remote = false) :
            base("library:components:ApplicationInsightsModule", name, args, options, remote)
        {
            var applicationInsights = new Component(name, new ComponentArgs
            {
                ResourceName = name,
                Location = args.ResourceGroupLocation,
                ResourceGroupName = args.ResourceGroupName,
                ApplicationType = args.Type.Apply(x => x),
                Kind = args.Type,
                FlowType = "Redfield",
                IngestionMode = IngestionMode.LogAnalytics,
                WorkspaceResourceId = args.LogAnalyticsWorkspaceId,
                PublicNetworkAccessForQuery = PublicNetworkAccessType.Enabled,
                PublicNetworkAccessForIngestion = PublicNetworkAccessType.Enabled,
                Tags =
                {
                    { "environment", Deployment.Instance.StackName }
                }
            });

            Id = applicationInsights.Id;
            InstrumentationKey = applicationInsights.InstrumentationKey;
        }
    }

    public sealed class ApplicationInsightsModuleArgs : ResourceArgs
    {
        [Input("type")]
        public Input<string> Type { get; set; }

        [Input("logAnalyticsWorkspaceId")]
        public Input<string> LogAnalyticsWorkspaceId { get; set; } = null!;

        [Input("resourceGroupName")]
        public Input<string> ResourceGroupName { get; }

        [Input("resourceGroupLocation")]
        public Input<string> ResourceGroupLocation { get; }

        public ApplicationInsightsModuleArgs(ResourceGroup resourceGroup)
        {
            ResourceGroupName = resourceGroup.Name;
            ResourceGroupLocation = resourceGroup.Location;
        }
    }
}
