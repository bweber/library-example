// ReSharper disable ObjectCreationAsStatement
using Pulumi;
using Pulumi.AzureNative.KeyVault;
using Pulumi.AzureNative.KeyVault.Inputs;
using Pulumi.AzureNative.Resources;
using Deployment = Pulumi.Deployment;
using ResourceArgs = Pulumi.ResourceArgs;

namespace Library.Infrastructure.Modules
{
    public class KeyVaultModule : ComponentResource
    {
        [Output("id")]
        public Output<string> Id { get; set; }

        [Output("name")]
        public Output<string> Name { get; set; }

        public KeyVaultModule(string name, KeyVaultModuleArgs args,
            ComponentResourceOptions options = null, bool remote = false) :
            base("library:components:KeyVaultModule", name, args, options, remote)
        {
            var keyVault = new Vault(name, new VaultArgs
            {
                ResourceGroupName = args.ResourceGroupName,
                Location = args.ResourceGroupLocation,
                Properties = new VaultPropertiesArgs
                {
                    TenantId = args.TenantId,
                    Sku = new SkuArgs
                    {
                        Family = SkuFamily.A,
                        Name = SkuName.Standard
                    },
                    SoftDeleteRetentionInDays = 90,
                    EnablePurgeProtection = true,
                    AccessPolicies = args.Policies
                },
                Tags =
                {
                    { "environment", Deployment.Instance.StackName }
                }
            });

            new DiagnosticSettingsModule($"{name}-diag", new DiagnosticSettingsModuleArgs
            {
                ResourceId = keyVault.Id,
                LogAnalyticsWorkspaceId = args.LogAnalyticsWorkspaceId,
                LogCategories =
                {
                    "AuditEvent"
                },
                MetricsCategories = new InputList<string>
                {
                    "AllMetrics"
                }
            });

            Id = keyVault.Id;
            Name = keyVault.Name;
        }
    }

    public sealed class KeyVaultModuleArgs : ResourceArgs
    {
        [Input("tenantId")]
        public Input<string> TenantId { get; init; }

        [Input("logAnalyticsWorkspaceId")]
        public Input<string> LogAnalyticsWorkspaceId { get; init; } = null!;

        [Input("resourceGroupName")]
        public Input<string> ResourceGroupName { get; }

        [Input("resourceGroupLocation")]
        public Input<string> ResourceGroupLocation { get; }

        [Input("policies")]
        public InputList<AccessPolicyEntryArgs> Policies { get; init; } = new();

        public KeyVaultModuleArgs(ResourceGroup resourceGroup)
        {
            ResourceGroupName = resourceGroup.Name;
            ResourceGroupLocation = resourceGroup.Location;
        }
    }
}
