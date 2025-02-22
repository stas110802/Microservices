FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Development
ENV ASPNETCORE_ENVIRONMENT=Development
WORKDIR /src
COPY ["src/Microservices.CommandsService/Microservices.CommandsService.csproj", "src/Microservices.CommandsService/"]
RUN dotnet restore "src/Microservices.CommandsService/Microservices.CommandsService.csproj"
COPY . .
WORKDIR "/src/src/Microservices.CommandsService"
RUN dotnet build "Microservices.CommandsService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Microservices.CommandsService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Microservices.CommandsService.dll", "--environment=Development"]
