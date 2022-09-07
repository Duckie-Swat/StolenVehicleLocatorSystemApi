FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["StolenVehicleLocatorSystem.Api/StolenVehicleLocatorSystem.Api.csproj", "StolenVehicleLocatorSystem.Api/"]
COPY ["StolenVehicleLocatorSystem.Business/StolenVehicleLocatorSystem.Business.csproj", "StolenVehicleLocatorSystem.Business/"]
COPY ["StolenVehicleLocatorSystem.DataAccessor/StolenVehicleLocatorSystem.DataAccessor.csproj", "StolenVehicleLocatorSystem.DataAccessor/"]
COPY ["StolenVehicleLocatorSystem.Contracts/StolenVehicleLocatorSystem.Contracts.csproj", "StolenVehicleLocatorSystem.Contracts/"]
COPY ["StolenVehicleLocatorSystem.Extensions/StolenVehicleLocatorSystem.Extensions.csproj", "StolenVehicleLocatorSystem.Extensions/"]
RUN dotnet restore "StolenVehicleLocatorSystem.Api/StolenVehicleLocatorSystem.Api.csproj"
COPY . .
WORKDIR "/src/StolenVehicleLocatorSystem.Api"
RUN dotnet build "StolenVehicleLocatorSystem.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StolenVehicleLocatorSystem.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StolenVehicleLocatorSystem.Api.dll"]