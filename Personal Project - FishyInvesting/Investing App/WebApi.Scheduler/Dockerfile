FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 1
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WebApi.Scheduler/WebApi.Scheduler.csproj", "WebApi.Scheduler/"]
RUN dotnet restore "WebApi.Scheduler/WebApi.Scheduler.csproj"
COPY . .
WORKDIR "/src/WebApi.Scheduler"
RUN dotnet build "WebApi.Scheduler.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "WebApi.Scheduler.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApi.Scheduler.dll"]
