FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["SalesMonitor.Api/SalesMonitor.Api.csproj", "SalesMonitor.Api/"]
RUN dotnet restore "SalesMonitor.Api/SalesMonitor.Api.csproj"
COPY . .
WORKDIR "/src/SalesMonitor.Api"
RUN dotnet build "SalesMonitor.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SalesMonitor.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SalesMonitor.Api.dll"]