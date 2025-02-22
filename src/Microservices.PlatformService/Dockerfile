FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
#ENV ASPNETCORE_ENVIRONMENT=Development
WORKDIR /src
COPY ["src/Microservices.PlatformService/Microservices.PlatformService.csproj", "src/Microservices.PlatformService/"]
RUN dotnet restore "src/Microservices.PlatformService/Microservices.PlatformService.csproj"
COPY . .
WORKDIR "/src/src/Microservices.PlatformService"
RUN dotnet build "Microservices.PlatformService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Microservices.PlatformService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Microservices.PlatformService.dll"]
# "--environment=Development"