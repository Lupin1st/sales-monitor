# Sales Monitor

## Run locally
- Clone the repository
- Open SalesMonitor.sln with an up to date version of VisualStudio 2019
- Select 'IIS Express' or 'SalesMonitor.Api' next to the green play symbol and press F5

## Run as a Linux Docker container locally
- Make sure Docker Desktop is installed on the development machine and Linux containers are selected.
    - https://www.docker.com/products/docker-desktop
- Select 'IIS Express' or 'SalesMonitor.Api' next to the green play symbol and press F5

## Configuration
The API can be configured to use an in memory datastore or CosmosDb as an alternative.
The selected repository will be filled with sample data for the last 30 days on application startup.

### Use the CosmosDb repository for local development
- Make sure the CosmosDb emulator is installed on the development machine
- Change the 'RepositoryKind' property inside 'appsettings.Development.json' from 'InMemory' to 'CosmosDb' 
```json
 "Repository": {
    "RepositoryKind": "CosmosDb",
    ...
  }
```

## Continuous integration
This project uses GitHub actions to create a docker image and hosts it inside an Azure AppService (Linux).
The hosted version of this API uses CosmosDb as its repository.

![CI](https://github.com/Lupin1st/sales-monitor/workflows/CI/badge.svg)

You can find the hosted version of this API at:
https://sales-monitor.azurewebsites.net

Info: The hosted Api uses the free hosing plan so it may take a while to coldstart.

## API documentation
The SwaggerUi API documentation is located at: https://sales-monitor.azurewebsites.net/swagger

The OpenApi (Swagger) definition is located at: https://sales-monitor.azurewebsites.net/swagger/v1/swagger.json
