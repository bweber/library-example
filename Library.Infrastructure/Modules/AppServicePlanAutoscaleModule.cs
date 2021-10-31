// ReSharper disable ObjectCreationAsStatement
using Pulumi;
using Pulumi.AzureNative.Insights;
using Pulumi.AzureNative.Insights.Inputs;
using Pulumi.AzureNative.Resources;
using Deployment = Pulumi.Deployment;
using ResourceArgs = Pulumi.ResourceArgs;

namespace Library.Infrastructure.Modules
{
    public class AppServicePlanAutoscaleModule : ComponentResource
    {
        public AppServicePlanAutoscaleModule(string name, AppServicePlanAutoscaleModuleArgs args,
            ComponentResourceOptions options = null, bool remote = false) :
            base("library:components:AppServicePlanAutoscaleModule", name, args, options, remote)
        {
            new AutoscaleSetting(name, new AutoscaleSettingArgs
            {
                Name = name,
                AutoscaleSettingName = name,
                Enabled = true,
                ResourceGroupName = args.ResourceGroupName,
                Location = args.ResourceGroupLocation,
                TargetResourceLocation = args.ResourceGroupLocation,
                TargetResourceUri = args.AppServicePlanId,
                Profiles =
                {
                    new AutoscaleProfileArgs
                    {
                        Name = "Autoscale Condition",
                        Capacity = new ScaleCapacityArgs
                        {
                            Default = args.MinimumInstanceCount.Apply(a => a.ToString()),
                            Maximum = args.MaximumInstanceCount.Apply(a => a.ToString()),
                            Minimum = args.MinimumInstanceCount.Apply(a => a.ToString()),
                        },
                        Rules =
                        {
                            new ScaleRuleArgs
                            {
                                ScaleAction = new ScaleActionArgs
                                {
                                    Direction = ScaleDirection.Increase,
                                    Type = ScaleType.ChangeCount,
                                    Value = "1",
                                    Cooldown = "PT5M"
                                },
                                MetricTrigger = new MetricTriggerArgs
                                {
                                    MetricName = "CpuPercentage",
                                    MetricNamespace = "microsoft.web/serverfarms",
                                    MetricResourceUri = args.AppServicePlanId,
                                    Operator = ComparisonOperationType.GreaterThan,
                                    Statistic = MetricStatisticType.Average,
                                    Threshold = args.CpuThresholdScaleUp,
                                    TimeAggregation = TimeAggregationType.Average,
                                    TimeGrain = "PT1M",
                                    TimeWindow = "PT10M",
                                    DividePerInstance = false
                                }
                            },
                            new ScaleRuleArgs
                            {
                                ScaleAction = new ScaleActionArgs
                                {
                                    Direction = ScaleDirection.Decrease,
                                    Type = ScaleType.ChangeCount,
                                    Value = "1",
                                    Cooldown = "PT5M"
                                },
                                MetricTrigger = new MetricTriggerArgs
                                {
                                    MetricName = "CpuPercentage",
                                    MetricNamespace = "microsoft.web/serverfarms",
                                    MetricResourceUri = args.AppServicePlanId,
                                    Operator = ComparisonOperationType.LessThan,
                                    Statistic = MetricStatisticType.Average,
                                    Threshold = args.CpuThresholdScaleDown,
                                    TimeAggregation = TimeAggregationType.Average,
                                    TimeGrain = "PT1M",
                                    TimeWindow = "PT10M",
                                    DividePerInstance = false
                                }
                            },
                            new ScaleRuleArgs
                            {
                                ScaleAction = new ScaleActionArgs
                                {
                                    Direction = ScaleDirection.Increase,
                                    Type = ScaleType.ChangeCount,
                                    Value = "1",
                                    Cooldown = "PT5M"
                                },
                                MetricTrigger = new MetricTriggerArgs
                                {
                                    MetricName = "MemoryPercentage",
                                    MetricNamespace = "microsoft.web/serverfarms",
                                    MetricResourceUri = args.AppServicePlanId,
                                    Operator = ComparisonOperationType.GreaterThan,
                                    Statistic = MetricStatisticType.Average,
                                    Threshold = args.MemoryThresholdScaleUp,
                                    TimeAggregation = TimeAggregationType.Average,
                                    TimeGrain = "PT1M",
                                    TimeWindow = "PT10M",
                                    DividePerInstance = false
                                }
                            },
                            new ScaleRuleArgs
                            {
                                ScaleAction = new ScaleActionArgs
                                {
                                    Direction = ScaleDirection.Decrease,
                                    Type = ScaleType.ChangeCount,
                                    Value = "1",
                                    Cooldown = "PT5M"
                                },
                                MetricTrigger = new MetricTriggerArgs
                                {
                                    MetricName = "MemoryPercentage",
                                    MetricNamespace = "microsoft.web/serverfarms",
                                    MetricResourceUri = args.AppServicePlanId,
                                    Operator = ComparisonOperationType.LessThan,
                                    Statistic = MetricStatisticType.Average,
                                    Threshold = args.MemoryThresholdScaleDown,
                                    TimeAggregation = TimeAggregationType.Average,
                                    TimeGrain = "PT1M",
                                    TimeWindow = "PT10M",
                                    DividePerInstance = false
                                }
                            }
                        }
                    }
                },
                Tags =
                {
                    { "environment", Deployment.Instance.StackName }
                }
            });
        }
    }

    public sealed class AppServicePlanAutoscaleModuleArgs : ResourceArgs
    {
        [Input("appServicePlanId")]
        public Input<string> AppServicePlanId { get; init; } = null!;

        [Input("minimumInstanceCount")]
        public Input<int> MinimumInstanceCount { get; init; } = 1;

        [Input("maximumInstanceCount")]
        public Input<int> MaximumInstanceCount { get; init; } = 10;

        [Input("cpuThresholdScaleUp")]
        public Input<double> CpuThresholdScaleUp { get; init; } = 80;

        [Input("cpuThresholdScaleDown")]
        public Input<double> CpuThresholdScaleDown { get; init; } = 40;

        [Input("memoryThresholdScaleUp")]
        public Input<double> MemoryThresholdScaleUp { get; init; } = 80;

        [Input("memoryThresholdScaleDown")]
        public Input<double> MemoryThresholdScaleDown { get; init; } = 40;

        [Input("resourceGroupName")]
        public Input<string> ResourceGroupName { get; }

        [Input("resourceGroupLocation")]
        public Input<string> ResourceGroupLocation { get; }

        public AppServicePlanAutoscaleModuleArgs(ResourceGroup resourceGroup)
        {
            ResourceGroupName = resourceGroup.Name;
            ResourceGroupLocation = resourceGroup.Location;
        }
    }
}
