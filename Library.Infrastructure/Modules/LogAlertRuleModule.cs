// ReSharper disable ObjectCreationAsStatement
using Pulumi;
using Pulumi.AzureNative.Insights;
using Pulumi.AzureNative.Insights.Inputs;
using Pulumi.AzureNative.Resources;
using Deployment = Pulumi.Deployment;
using ResourceArgs = Pulumi.ResourceArgs;

namespace Library.Infrastructure.Modules
{
    public class LogAlertRuleModule : ComponentResource
    {
        public LogAlertRuleModule(string name, LogAlertRuleModuleArgs args, ComponentResourceOptions options = null,
            bool remote = false) : base("library:components:LogAlertRuleModule", name, args, options, remote)
        {
            new ScheduledQueryRule(name,
                new ScheduledQueryRuleArgs
                {
                    RuleName = args.RuleName,
                    Location = args.ResourceGroupLocation,
                    ResourceGroupName = args.ResourceGroupName,
                    Action = new AlertingActionArgs
                    {
                        AznsAction = new AzNsActionGroupArgs
                        {
                            ActionGroup = new InputList<string> { args.ActionGroup },
                            EmailSubject = args.AlertEmailSubject,
                        },
                        OdataType = "Microsoft.WindowsAzure.Management.Monitoring.Alerts.Models.Microsoft." +
                                    "AppInsights.Nexus.DataContracts.Resources.ScheduledQueryRules.AlertingAction",
                        Severity = args.AlertSeverity,
                        Trigger = new TriggerConditionArgs
                        {
                            Threshold = args.TriggerThreshold,
                            ThresholdOperator = args.TriggerThresholdOperator
                        }
                    },
                    Description = args.Description,
                    Enabled = args.Enabled,
                    AutoMitigate = args.AutoMitigate,
                    Schedule = new ScheduleArgs
                    {
                        FrequencyInMinutes = args.ScheduleFrequencyInMinutes,
                        TimeWindowInMinutes = args.ScheduleTimeWindowInMinutes
                    },
                    Source = new SourceArgs
                    {
                        DataSourceId = args.DataSourceId,
                        Query = args.LogQuery,
                        QueryType = QueryType.ResultCount
                    },
                    Tags =
                    {
                        { "environment", Deployment.Instance.StackName }
                    }
                });
        }
    }

    public sealed class LogAlertRuleModuleArgs : ResourceArgs
    {
        [Input("ruleName", true)]
        public Input<string> RuleName { get; init; } = null!;

        [Input("actionGroup", true)]
        public Input<string> ActionGroup { get; init; } = null!;

        [Input("alertEmailSubject")]
        public Input<string> AlertEmailSubject { get; init; } = null;

        [Input("alertSeverity")]
        public InputUnion<string, AlertSeverity> AlertSeverity { get; init; } =
            Pulumi.AzureNative.Insights.AlertSeverity.Three;

        [Input("description", true)]
        public Input<string> Description { get; init; } = null!;

        [Input("enabled")]
        public InputUnion<string, Enabled> Enabled { get; init; } =
            Pulumi.AzureNative.Insights.Enabled.True;

        [Input("autoMitigate")]
        public Input<bool> AutoMitigate { get; init; } = false;

        [Input("dataSourceId", true)]
        public Input<string> DataSourceId { get; init; } = null!;

        [Input("triggerThreshold", true)]
        public Input<double> TriggerThreshold { get; init; } = null!;

        [Input("triggerThresholdOperator")]
        public InputUnion<string, ConditionalOperator> TriggerThresholdOperator { get; init; } =
            ConditionalOperator.GreaterThan;

        [Input("scheduleFrequencyInMinutes")]
        public Input<int> ScheduleFrequencyInMinutes { get; init; } = 5;

        [Input("scheduleTimeWindowInMinutes")]
        public Input<int> ScheduleTimeWindowInMinutes { get; init; } = 5;

        [Input("logQuery", true)]
        public Input<string> LogQuery { get; init; } = null!;

        [Input("resourceGroupName")]
        public Input<string> ResourceGroupName { get; }

        [Input("resourceGroupLocation")]
        public Input<string> ResourceGroupLocation { get; }

        public LogAlertRuleModuleArgs(ResourceGroup resourceGroup)
        {
            ResourceGroupName = resourceGroup.Name;
            ResourceGroupLocation = resourceGroup.Location;
        }
    }
}
