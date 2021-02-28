using Library.Infrastructure.Modules;
using Pulumi;
using Pulumi.Azure.AppService;
using Pulumi.Azure.AppService.Inputs;
using Pulumi.Azure.Core;
using Pulumi.Azure.KeyVault;
using Pulumi.Azure.KeyVault.Inputs;

// ReSharper disable ObjectCreationAsStatement

namespace Library.Infrastructure
{
    public class APIStack : Stack
    {
        [Output("appServiceName")]
        public Output<string> AppServiceName { get; set; }
        
        [Output("appServiceUrl")]
        public Output<string> Endpoint { get; set; }
        
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

            var keyVault = CreateKeyVault(resourceGroup, config.Require("keyvaultSku"), tenantId, currentPrincipalId);
            var appServicePlan = CreateAppServicePlan(resourceGroup, config);

            var appService = new AppServiceModule("library-app", new AppServiceModuleArgs(resourceGroup)
            {
                AspnetEnvironment = config.Require("aspnetEnvironment"),
                AppServicePlanId = appServicePlan.Id,
                TenantId = tenantId,
                KeyVaultId = keyVault.Id,
                KeyVaultName = keyVault.Name
            });

            new SqlDatabaseModule("library-sql", new SqlDatabaseModuleArgs(resourceGroup)
            {
                DatabaseSize = config.Require("databaseSize"),
                KeyVaultId = keyVault.Id,
            });

            AppServiceName = appService.Name;
            Endpoint = Output.Format($"https://{appService.DefaultSiteHostname}");
        }

        private static Plan CreateAppServicePlan(ResourceGroup resourceGroup, Config config)
        {
            return new("library-plan", new PlanArgs
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
        }

        private static KeyVault CreateKeyVault(ResourceGroup resourceGroup, string keyVaultSku, Output<string> tenantId, Output<string> currentPrincipalId)
        {
            return new("library-kv", new KeyVaultArgs
            {
                ResourceGroupName = resourceGroup.Name,
                SkuName = keyVaultSku,
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
        }
    }
}