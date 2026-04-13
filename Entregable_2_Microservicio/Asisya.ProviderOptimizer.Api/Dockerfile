FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Asisya.ProviderOptimizer.Api/Asisya.ProviderOptimizer.Api.csproj", "Asisya.ProviderOptimizer.Api/"]
COPY ["Asisya.ProviderOptimizer.Application/Asisya.ProviderOptimizer.Application.csproj", "Asisya.ProviderOptimizer.Application/"]
COPY ["Asisya.ProviderOptimizer.Domain/Asisya.ProviderOptimizer.Domain.csproj", "Asisya.ProviderOptimizer.Domain/"]
COPY ["Asisya.ProviderOptimizer.Infrastructure/Asisya.ProviderOptimizer.Infrastructure.csproj", "Asisya.ProviderOptimizer.Infrastructure/"]
RUN dotnet restore "Asisya.ProviderOptimizer.Api/Asisya.ProviderOptimizer.Api.csproj"
COPY . .
WORKDIR "/src/Asisya.ProviderOptimizer.Api"
RUN dotnet build "Asisya.ProviderOptimizer.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Asisya.ProviderOptimizer.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Asisya.ProviderOptimizer.Api.dll"]
