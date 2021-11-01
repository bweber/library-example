// ReSharper disable ObjectCreationAsStatement
using Library.Infrastructure.Modules;
using Pulumi;
using Pulumi.AzureNative.Authorization;
using Pulumi.AzureNative.Insights;
using Pulumi.AzureNative.Insights.Inputs;
using Pulumi.AzureNative.KeyVault;
using Pulumi.AzureNative.KeyVault.Inputs;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Web;
using ActionGroupArgs = Pulumi.AzureNative.Insights.ActionGroupArgs;
using Deployment = Pulumi.Deployment;

namespace Library.Infrastructure
{
    public class APIStack : Stack
    {
        public APIStack()
        {
            var stackName = Deployment.Instance.StackName;
            var config = new Config();

            var clientConfig = Output.Create(GetClientConfig.InvokeAsync());
            var currentPrincipalId = clientConfig.Apply(c => c.ObjectId);
            var tenantId = clientConfig.Apply(c => c.TenantId);

            var resourceGroup = new ResourceGroup("library-rg", new ResourceGroupArgs
            {
                ResourceGroupName = "library-rg",
                Tags =
                {
                    { "environment", stackName }
                }
            });

            var logAnalyticsWorkspace = new LogAnalyticsWorkspaceModule("library-logs",
                new LogAnalyticsWorkspaceModuleArgs(resourceGroup)
                {
                    LogRetentionDays = config.RequireInt32("logRetentionDays")
                });

            var applicationInsights = new ApplicationInsightsModule("library-app-insights",
                new ApplicationInsightsModuleArgs(resourceGroup));

            var appServicePlan = new AppServicePlanModule("library-plan", new AppServicePlanModuleArgs(resourceGroup)
            {
                LogAnalyticsWorkspaceId = logAnalyticsWorkspace.Id,
                AppServicePlanTier = config.Require("appServicePlanTier"),
                AppServicePlanSize = config.Require("appServicePlanSize")
            });

            new AppServicePlanAutoscaleModule("library-plan-autoscale",
                new AppServicePlanAutoscaleModuleArgs(resourceGroup)
                {
                    AppServicePlanId = appServicePlan.Id,
                    MinimumInstanceCount = config.RequireInt32("appServiceCapacity")
                });

            var appService = new AppServiceModule("library-app", new AppServiceModuleArgs(resourceGroup)
            {
                LogAnalyticsWorkspaceId = logAnalyticsWorkspace.Id,
                AppServicePlanId = appServicePlan.Id
            });

            var keyVault = new KeyVaultModule("library-vault", new KeyVaultModuleArgs(resourceGroup)
            {
                LogAnalyticsWorkspaceId = logAnalyticsWorkspace.Id,
                TenantId = tenantId,
                Policies =
                {
                    new AccessPolicyEntryArgs
                    {
                        TenantId = tenantId,
                        ObjectId = currentPrincipalId,
                        Permissions = new PermissionsArgs
                        {
                            Secrets =  { "get", "list", "set", "delete", "recover", "purge" }
                        }
                    },
                    new AccessPolicyEntryArgs
                    {
                        TenantId = tenantId,
                        ObjectId = appService.PrincipalId,
                        Permissions = new PermissionsArgs
                        {
                            Secrets =  { "get", "list" }
                        }
                    }
                }
            });

            new WebAppApplicationSettings("library-app-settings",
                new WebAppApplicationSettingsArgs
                {
                    Name = appService.Name,
                    ResourceGroupName = resourceGroup.Name,
                    Properties =
                    {
                        { "WEBSITES_ENABLE_APP_SERVICE_STORAGE", "false" },
                        { "WEBSITES_CONTAINER_START_TIME_LIMIT", "1800" },
                        { "ASPNETCORE_ENVIRONMENT", config.Require("aspnetEnvironment") },
                        { "APPINSIGHTS_INSTRUMENTATIONKEY", applicationInsights.InstrumentationKey },
                        { "KeyVaultUri", Output.Format($"https://{keyVault.Name}.vault.azure.net/") }
                    }
                });

            var sqlDatabase = new SqlDatabaseModule("library-app-sql", new SqlDatabaseModuleArgs(resourceGroup)
            {
                LogAnalyticsWorkspaceId = logAnalyticsWorkspace.Id,
                DatabaseSize = config.Require("databaseSize")
            });

            new Secret("library-app-sql-connection-string", new SecretArgs
            {
                ResourceGroupName = resourceGroup.Name,
                SecretName = "ConnectionStrings--LibraryDB",
                VaultName = keyVault.Name,
                Properties = new SecretPropertiesArgs
                {
                    Value = sqlDatabase.ConnectionString
                },
                Tags =
                {
                    { "environment", Deployment.Instance.StackName }
                }
            });

            SetupAlerts(resourceGroup, config.Require("administratorEmail"), applicationInsights);
        }

        private static void SetupAlerts(ResourceGroup resourceGroup, string administratorEmail,
            ApplicationInsightsModule applicationInsights)
        {
            var actionGroup = new ActionGroup("library-alert-action-group", new ActionGroupArgs
            {
                ActionGroupName = "library-alert-action-group",
                Enabled = true,
                GroupShortName = "library-ag",
                Location = "Global",
                ResourceGroupName = resourceGroup.Name,
                EmailReceivers =
                {
                    new EmailReceiverArgs
                    {
                        Name = "Admininistrator Email",
                        EmailAddress = administratorEmail,
                        UseCommonAlertSchema = false,
                    }
                }
            });

            new LogAlertRuleModule("library-app-5XX", new LogAlertRuleModuleArgs(resourceGroup)
            {
                RuleName = "library-app-5XX",
                Description = "Library App HTTP 5XX Alert",
                ActionGroup = actionGroup.Id,
                AlertSeverity = AlertSeverity.One,
                LogQuery = "requests | where toint(resultCode) >= 500",
                TriggerThreshold = 5,
                TriggerThresholdOperator = ConditionalOperator.GreaterThan,
                ScheduleFrequencyInMinutes = 5,
                ScheduleTimeWindowInMinutes = 5,
                DataSourceId = applicationInsights.Id
            });
        }
    }
}
