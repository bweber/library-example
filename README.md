# Pulumi Example

An example application showing how to use Pulumi, Docker, and Github Actions

### Dependencies
- [Docker](https://www.docker.com/products/docker-desktop)
- [Docker Compose](https://www.docker.com/products/docker-desktop)
- [.NET 5](https://dotnet.microsoft.com/download/dotnet/5.0)
- [.NET EF Tools](https://docs.microsoft.com/en-us/ef/core/cli/dotnet)

### Run Acceptance Tests
`make verify`

### Debugging Locally
#### Option 1
1. Run `make start-local`
1. Launch the application via Rider/Visual Studio/VS Code

#### Option 2
1. Run `make verify`
1. Run `make stop-service` when complete
1. Launch the application via Rider/Visual Studio/VS Code