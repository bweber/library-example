using System.Collections.Generic;
using Newtonsoft.Json;

namespace Library.Infrastructure.Tests
{
    public class MockConfig
    {
        [JsonProperty("library-api:keyvaultSku")]
        public string KeyVaultSku { get; set; }

        [JsonProperty("library-api:appServicePlanTier")]
        public string AppServicePlanTier { get; set; }
        
        [JsonProperty("library-api:appServicePlanSize")]
        public string AppServicePlanSize { get; set; }
        
        [JsonProperty("library-api:appServiceCapacity")]
        public int AppServiceCapacity { get; set; }
        
        [JsonProperty("library-api:aspnetEnvironment")]
        public string AspnetEnvironment { get; set; }

        [JsonProperty("library-api:databaseSize")]
        public string DatabaseSize { get; set; }
    }
}