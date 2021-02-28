using System;
using Pulumi;
using Pulumi.Azure.AppInsights;
using Pulumi.Azure.AppService;
using Pulumi.Azure.AppService.Inputs;
using Pulumi.Azure.Core;
using Pulumi.Azure.KeyVault;
using Pulumi.Azure.KeyVault.Inputs;
using Pulumi.Azure.Sql;
using Pulumi.Random;

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
                Tags =
                {
                    { "environment", stackName }
                }
            });

            var vault = new KeyVault("library-kv", new KeyVaultArgs
            {
                ResourceGroupName = resourceGroup.Name,
                SkuName = config.Require("keyvaultSku"),
                TenantId = tenantId,
                SoftDeleteRetentionDays = 90,
                PurgeProtectionEnabled = true,
                AccessPolicies =
                {
                    new KeyVaultAccessPolicyArgs
                    {
                        TenantId = tenantId,
                        ObjectId = currentPrincipalId,
                        SecretPermissions = { "delete", "get", "list", "set", "recover", "purge" }
                    }
                },
                Tags =
                {
                    { "environment", Deployment.Instance.StackName }
                }
            });
            
            var appServicePlan = new Plan("library-plan", new PlanArgs
            {
                Location = resourceGroup.Location,
                ResourceGroupName = resourceGroup.Name,
                Kind = "Linux",
                Reserved = true,
                Sku = new PlanSkuArgs
                {
                    Tier = config.Require("appServicePlanTier"),
                    Size = config.Require("appServicePlanSize"),
                    Capacity = config.RequireInt32("appServiceCapacity")
                }, 
                Tags =
                {
                    { "environment", Deployment.Instance.StackName }
                }
            });
            
            var appInsights = new Insights("library-insights", new InsightsArgs
            {
                Location = resourceGroup.Location,
                ResourceGroupName = resourceGroup.Name,
                ApplicationType = "web",
                Tags =
                {
                    { "environment", Deployment.Instance.StackName }
                }
            });
            
            var appService = new AppService("library-app", new AppServiceArgs
            {
                Location = resourceGroup.Location,
                ResourceGroupName = resourceGroup.Name,
                AppServicePlanId = appServicePlan.Id,
                HttpsOnly = true,
                Identity = new AppServiceIdentityArgs
                {
                    Type = "SystemAssigned"
                },
                AppSettings =
                {
                    { "WEBSITES_ENABLE_APP_SERVICE_STORAGE", "false" },
                    { "ASPNETCORE_ENVIRONMENT", config.Require("aspnetEnvironment") },
                    { "APPINSIGHTS_INSTRUMENTATIONKEY", appInsights.InstrumentationKey },
                    { "KeyVaultName", vault.Name }
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
            
            var appAccessPolicy = new AccessPolicy("library-app-policy", new AccessPolicyArgs
            {
                KeyVaultId = vault.Id,
                TenantId = tenantId,
                ObjectId = appServicePrincipalId,
                SecretPermissions = { "get", "list" }
            });

            var adminUsername = new RandomString("library-sql-admin-username", new RandomStringArgs
            {
                Length = 16,
                Special = false,
                Number = false
            });
            
            var adminPassword = new RandomPassword("library-sql-admin-pass", new RandomPasswordArgs
            {
                Length = 32, 
                Special = true
            });
            
            var sqlServer = new SqlServer("library-sqlserver", new SqlServerArgs
            {
                ResourceGroupName = resourceGroup.Name,
                Location = resourceGroup.Location,
                AdministratorLogin = adminUsername.Result,
                AdministratorLoginPassword = adminPassword.Result,
                Version = "12.0",
                Tags =
                {
                    { "environment", Deployment.Instance.StackName }
                }
            });

            var database = new Database("library-database", new DatabaseArgs
            {
                Name = "Library",
                ResourceGroupName = resourceGroup.Name,
                Location = resourceGroup.Location,
                ServerName = sqlServer.Name,
                RequestedServiceObjectiveName = config.Require("databaseSize"),
                Tags =
                {
                    { "environment", Deployment.Instance.StackName }
                }
            });
            
            var connectionSecret = new Secret("library-sql-connection-string", new SecretArgs
            {
                Name = "ConnectionStrings--LibraryDB",
                KeyVaultId = vault.Id,
                Value = Output.Format($"Server=tcp:{sqlServer.Name}.database.windows.net;Database={database.Name};"),
                Tags =
                {
                    { "environment", Deployment.Instance.StackName }
                }
            });
            
            var librarySqlAdmin = new ActiveDirectoryAdministrator("library-admin", new ActiveDirectoryAdministratorArgs
            {
                ResourceGroupName = resourceGroup.Name,
                TenantId = tenantId,
                ObjectId = appServicePrincipalId,
                Login = "libraryadmin",
                ServerName = sqlServer.Name
            });
        }
    }
}