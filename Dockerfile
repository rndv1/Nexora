FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Nexora.slnx .
COPY Nexora.API/Nexora.API.csproj Nexora.API/
COPY Nexora.Application/Nexora.Application.csproj Nexora.Application/
COPY Nexora.Domain/Nexora.Domain.csproj Nexora.Domain/
COPY Nexora.Infrastructure/Nexora.Infrastructure.csproj Nexora.Infrastructure/
COPY Nexora.TaxInspection.Worker/Nexora.TaxInspection.Worker.csproj Nexora.TaxInspection.Worker/
COPY Nexora.TaxInspection.Application/Nexora.TaxInspection.Application.csproj Nexora.TaxInspection.Application/
COPY Nexora.TaxInspection.Domain/Nexora.TaxInspection.Domain.csproj Nexora.TaxInspection.Domain/
COPY Nexora.TaxInspection.Infrastructure/Nexora.TaxInspection.Infrastructure.csproj Nexora.TaxInspection.Infrastructure/

RUN dotnet restore Nexora.slnx

COPY . .
WORKDIR /src/Nexora.API
RUN dotnet publish Nexora.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Nexora.API.dll"]