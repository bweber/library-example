using Pulumi;
using Pulumi.AzureNative.Insights;
using Pulumi.AzureNative.Resources;
using Deployment = Pulumi.Deployment;
using ResourceArgs = Pulumi.ResourceArgs;

namespace Library.Infrastructure.Modules
{
    public class ApplicationInsightsModule : ComponentResource
    {
        [Output("Id")]
        public Output<string> Id { get; }

        [Output("instrumentationKey")]
        public Output<string> InstrumentationKey { get; }

        public ApplicationInsightsModule(string name, ApplicationInsightsModuleArgs args,
            ComponentResourceOptions options = null, bool remote = false) :
            base("library:components:ApplicationInsightsModule", name, args, options, remote)
        {
            var applicationInsights = new Component(name, new ComponentArgs
            {
                ResourceName = args.ResourceGroupName,
                Location = args.ResourceGroupLocation,
                ApplicationType = "web",
                Kind = "web",
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
        [Input("resourceGroupName")]
        public Input<string> ResourceGroupName { get; set; }

        [Input("resourceGroupLocation")]
        public Input<string> ResourceGroupLocation { get; set; }

        public ApplicationInsightsModuleArgs(ResourceGroup resourceGroup)
        {
            ResourceGroupName = resourceGroup.Name;
            ResourceGroupLocation = resourceGroup.Location;
        }
    }
}
