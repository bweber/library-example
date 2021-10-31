// ReSharper disable ObjectCreationAsStatement
using Pulumi;
using Pulumi.AzureNative.Insights;
using Pulumi.AzureNative.Insights.Inputs;
using System.Linq;

namespace Library.Infrastructure.Modules
{
    public class DiagnosticSettingsModule : ComponentResource
    {
        public DiagnosticSettingsModule(string name, DiagnosticSettingsModuleArgs args,
            ComponentResourceOptions options = null, bool remote = false) :
            base("library:components:DiagnosticSettingsModule", name, args, options, remote)
        {
            new DiagnosticSetting(name, new DiagnosticSettingArgs
            {
                Name = name,
                ResourceUri = args.ResourceId,
                WorkspaceId = args.LogAnalyticsWorkspaceId,
                Logs = args.LogCategories.Apply(logCategories =>
                    logCategories.Select(x => new LogSettingsArgs
                    {
                        Category = x,
                        Enabled = true,
                        RetentionPolicy = new RetentionPolicyArgs
                        {
                            Enabled = true,
                            Days = args.LogRetentionDays
                        }
                    }).ToList()),
                Metrics = args.MetricsCategories.Apply(metricsCategories =>
                    metricsCategories.Select(x => new MetricSettingsArgs
                    {
                        Category = x,
                        Enabled = true,
                        RetentionPolicy = new RetentionPolicyArgs
                        {
                            Enabled = true,
                            Days = args.MetricRetentionDays
                        }
                    }).ToList())
            });
        }
    }

    public sealed class DiagnosticSettingsModuleArgs : ResourceArgs
    {
        [Input("resourceId")]
        public Input<string> ResourceId { get; init; }

        [Input("logRetentionDays")]
        public Input<int> LogRetentionDays { get; init; } = 90;

        [Input("metricRetentionDays")]
        public Input<int> MetricRetentionDays { get; init; } = 30;

        [Input("logAnalyticsWorkspaceId")]
        public Input<string> LogAnalyticsWorkspaceId { get; init; }

        [Input("logCategories")]
        public InputList<string> LogCategories { get; init; } = new();

        [Input("metricsCategories")]
        public InputList<string> MetricsCategories { get; init; } = new();
    }
}
