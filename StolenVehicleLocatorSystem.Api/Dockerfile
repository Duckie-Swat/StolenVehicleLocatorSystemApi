#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["StolenVehicleLocatorSystem.Api/StolenVehicleLocatorSystem.Api.csproj", "StolenVehicleLocatorSystem.Api/"]
COPY ["StolenVehicleLocatorSystem.Business/StolenVehicleLocatorSystem.Business.csproj", "StolenVehicleLocatorSystem.Business/"]
COPY ["StolenVehicleLocatorSystem.DataAccessor/StolenVehicleLocatorSystem.DataAccessor.csproj", "StolenVehicleLocatorSystem.DataAccessor/"]
COPY ["StolenVehicleLocatorSystem.Contracts/StolenVehicleLocatorSystem.Contracts.csproj", "StolenVehicleLocatorSystem.Contracts/"]
RUN dotnet restore "StolenVehicleLocatorSystem.Api/StolenVehicleLocatorSystem.Api.csproj"
COPY . .
WORKDIR "/src/StolenVehicleLocatorSystem.Api"
RUN dotnet build "StolenVehicleLocatorSystem.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StolenVehicleLocatorSystem.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StolenVehicleLocatorSystem.Api.dll"]