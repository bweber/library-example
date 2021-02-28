using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Pulumi;
using Pulumi.Testing;

namespace Library.Infrastructure.Tests
{
    internal class MockStack : IMocks
    {
        private readonly Dictionary<string, object> _clientConfig;

        public MockStack(Dictionary<string, object> clientConfig)
        {
            _clientConfig = clientConfig;
        }

        public Task<(string id, object state)> NewResourceAsync(
            string type, string name, ImmutableDictionary<string, object> inputs, string provider, string id)
        {
            var outputs = ImmutableDictionary.CreateBuilder<string, object>();

            outputs.AddRange(inputs);
            
            if (!inputs.ContainsKey("name"))
                outputs.Add("name", name);
            
            switch (type)
            {
                case "azure:core/resourceGroup:ResourceGroup":
                    outputs.Add("location", "CentralUS");
                    break;
                case "azure:appinsights/insights:Insights":
                    outputs.Add("instrumentationKey", Guid.NewGuid().ToString());
                    break;
                case "azure:appservice/appService:AppService":
                {
                    var identity = (ImmutableDictionary<string, object>) outputs["identity"];
                    var identityCopy = ImmutableDictionary.CreateBuilder<string, object>();

                    identityCopy.AddRange(identity);
                    identityCopy.Add("principalId", "MyAppIdentity");

                    outputs["identity"] = identityCopy;
                    break;
                }
                case "random:index/randomString:RandomString":
                case "random:index/randomPassword:RandomPassword":
                    outputs.Add("result", Guid.NewGuid().ToString());
                    break;
            }

            return Task.FromResult(($"{name}_id", (object)outputs));
        }

        public Task<object> CallAsync(string token, ImmutableDictionary<string, object> inputs, string provider)
        {
            if (token == "azure:core/getClientConfig:getClientConfig")
            {
                return Task.FromResult((object) _clientConfig);
            }
            
            return Task.FromResult((object)inputs);
        }
    }
    
    internal class MockStackHelper
    {
        public readonly MockConfig Config = new()
        {
            KeyVaultSku = "standard",
            AppServicePlanTier = "Basic",
            AppServicePlanSize = "B1",
            AppServiceCapacity = 1,
            DatabaseSize = "S0",
            AspnetEnvironment = "Development"
        };
        public readonly Dictionary<string, object> ClientConfig = new()
        {
            { "objectId", Guid.NewGuid().ToString() },
            { "tenantId", Guid.NewGuid().ToString() }
        };

        public Task<ImmutableArray<Resource>> TestAsync()
        {
            Environment.SetEnvironmentVariable("PULUMI_CONFIG", JsonConvert.SerializeObject(Config));
            
            return Deployment.TestAsync<APIStack>(new MockStack(ClientConfig), new TestOptions { IsPreview = false, StackName = "dev", ProjectName = "library-api" });
        }
    }

    internal static class MOckStackExtensions
    {
        public static Task<T> GetValueAsync<T>(this Output<T> output)
        {
            var tcs = new TaskCompletionSource<T>();
            output.Apply(v =>
            {
                tcs.SetResult(v);
                return v;
            });
            return tcs.Task;
        }
    }
}