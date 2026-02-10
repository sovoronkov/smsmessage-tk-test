ARG CI_REGISTRY_IMAGE_IMG
#FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
FROM $CI_REGISTRY_IMAGE_IMG/dotnet/aspnet:8.0.4  AS base

USER app
WORKDIR /app

#FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
FROM $CI_REGISTRY_IMAGE_IMG/dotnet/sdk:8.0.204 AS build

ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Sms.Api/Sms.Api.csproj", "Sms.Api/"]
RUN dotnet restore "./Sms.Api/Sms.Api.csproj"
COPY . .
WORKDIR "/src/Sms.Api"
RUN dotnet build "./Sms.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Sms.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ARG ASPNETCORE_ENVIRONMENT
ENV ASPNETCORE_ENVIRONMENT=${ASPNETCORE_ENVIRONMENT}
ENTRYPOINT ["dotnet", "Sms.Api.dll"]
