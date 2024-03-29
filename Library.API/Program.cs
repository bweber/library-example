using System;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Library.Common.Extensions;
using Library.Common.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Library.API
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseDefaultServiceProvider((context, options) =>
                {
                    var isDevelopment = context.HostingEnvironment.IsDevelopment() ||
                                        context.HostingEnvironment.IsAcceptance();
                    options.ValidateScopes = isDevelopment;
                    options.ValidateOnBuild = isDevelopment;
                })
                .ConfigureAppConfiguration((context, config) =>
                {
                    if (context.HostingEnvironment.IsAcceptance()) return;

                    var builtConfig = config.Build();
                    var secretClient = new SecretClient(
                        new Uri(builtConfig.GetValue<string>("KeyVaultUri")),
                        new DefaultAzureCredential());

                    config.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
                })
                .ConfigureLogging()
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}
