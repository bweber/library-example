# Pulumi Example

An example application showing how to use Pulumi, Docker, and Github Actions

[![Verify](https://github.com/bweber/library-example/actions/workflows/verify.yml/badge.svg)](https://github.com/bweber/library-example/actions/workflows/verify.yml)

## Dependencies
- [Docker](https://www.docker.com/products/docker-desktop)
- [Docker Compose](https://www.docker.com/products/docker-desktop)
- [.NET 5](https://dotnet.microsoft.com/download/dotnet/5.0)
- [.NET EF Tools](https://docs.microsoft.com/en-us/ef/core/cli/dotnet)
- [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)
- [Pulumi](https://www.pulumi.com/docs/get-started/install/)

## Run Acceptance Tests
`make verify`

## Debugging Locally
#### Option 1
1. Run `make start-local`
1. Launch the application via Rider/Visual Studio/VS Code

#### Option 2
1. Run `make verify`
1. Run `make stop-service` when complete
1. Launch the application via Rider/Visual Studio/VS Code

## Pulumi Setup
1. Ensure you have Pulumi installed via the dependencies list above
1. Create a new account on [Pulumi](https://www.pulumi.com/)
1. Select or create an organization from the dropdown
1. Go to `Settings` tab
1. Go to `Access Tokens` page
1. Create a new access token
1. Copy the generated access token
1. Create a new Github secret called `PULUMI_ACCESS_TOKEN` with the copied value
1. Add the Pulumi Bot to your Github org: https://www.pulumi.com/docs/guides/continuous-delivery/github-actions/

## Azure Setup
1. Ensure you have Azure CLI installed via the dependencies list above
1. Run `az login --tenant <TENANT_ID>`
    1. Get this value from Azure AD Portal
1. Create Service Principal `az ad sp create-for-rbac -n "InfraCreator" --sdk-auth --role "Contributor"`
    1. Example output should look like:
    ```
   {
        "clientId": "<ARM_CLIENT_ID>",
        "clientSecret": "<ARM_CLIENT_SECRET>",
        "subscriptionId": "<ARM_SUBSCRIPTION_ID>",
        "tenantId": "<ARM_TENANT_ID>",
        "activeDirectoryEndpointUrl": "https://login.microsoftonline.com",
        "resourceManagerEndpointUrl": "https://management.azure.com/",
        "activeDirectoryGraphResourceId": "https://graph.windows.net/",
        "sqlManagementEndpointUrl": "https://management.core.windows.net:8443/",
        "galleryEndpointUrl": "https://gallery.azure.com/",
        "managementEndpointUrl": "https://management.core.windows.net/"
    }
    ```
1. Copy output to Github as a secret called `AZURE_CREDENTIALS`
1. Copy the four parts of the output to Github secrets with respective names:
    1. `ARM_CLIENT_ID`
    1. `ARM_CLIENT_SECRET`
    1. `ARM_SUBSCRIPTION_ID`
    1. `ARM_TENANT_ID`