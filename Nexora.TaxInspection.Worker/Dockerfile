FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Nexora.TaxInspection.Worker/Nexora.TaxInspection.Worker.csproj", "Nexora.TaxInspection.Worker/"]
COPY ["Nexora.TaxInspection.Application/Nexora.TaxInspection.Application.csproj", "Nexora.TaxInspection.Application/"]
COPY ["Nexora.TaxInspection.Domain/Nexora.TaxInspection.Domain.csproj", "Nexora.TaxInspection.Domain/"]
COPY ["Nexora.TaxInspection.Infrastructure/Nexora.TaxInspection.Infrastructure.csproj", "Nexora.TaxInspection.Infrastructure/"]
RUN dotnet restore "Nexora.TaxInspection.Worker/Nexora.TaxInspection.Worker.csproj"

COPY . .
WORKDIR "/src/Nexora.TaxInspection.Worker"
RUN dotnet build "Nexora.TaxInspection.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Nexora.TaxInspection.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Nexora.TaxInspection.Worker.dll"]
