// ReSharper disable ObjectCreationAsStatement
using Pulumi;
using Pulumi.AzureNative.OperationalInsights;
using Pulumi.AzureNative.OperationalInsights.Inputs;
using Pulumi.AzureNative.Resources;
using Deployment = Pulumi.Deployment;
using ResourceArgs = Pulumi.ResourceArgs;

namespace Library.Infrastructure.Modules
{
    public class LogAnalyticsWorkspaceModule : ComponentResource
    {
        [Output("id")]
        public Output<string> Id { get; }

        public LogAnalyticsWorkspaceModule(string name, LogAnalyticsWorkspaceModuleArgs args,
            ComponentResourceOptions options = null, bool remote = false) :
            base("library:components:LogAnalyticsWorkspaceModule", name, args, options, remote)
        {
            var workspace = new Workspace(name, new WorkspaceArgs
            {
                WorkspaceName = name,
                Location = args.ResourceGroupLocation,
                ResourceGroupName = args.ResourceGroupName,
                RetentionInDays = args.LogRetentionDays, //config.RequireInt32("logRetentionDays"),
                PublicNetworkAccessForIngestion = PublicNetworkAccessType.Enabled,
                PublicNetworkAccessForQuery = PublicNetworkAccessType.Enabled,
                Sku = new WorkspaceSkuArgs
                {
                    Name = "PerGB2018",
                },
                Tags =
                {
                    { "environment", Deployment.Instance.StackName }
                }
            });

            Id = workspace.Id;
        }
    }

    public sealed class LogAnalyticsWorkspaceModuleArgs : ResourceArgs
    {
        [Input("logRetentionDays")]
        public Input<int> LogRetentionDays { get; set; }

        [Input("resourceGroupName")]
        public Input<string> ResourceGroupName { get; set; }

        [Input("resourceGroupLocation")]
        public Input<string> ResourceGroupLocation { get; set; }

        public LogAnalyticsWorkspaceModuleArgs(ResourceGroup resourceGroup)
        {
            ResourceGroupName = resourceGroup.Name;
            ResourceGroupLocation = resourceGroup.Location;
        }
    }
}
