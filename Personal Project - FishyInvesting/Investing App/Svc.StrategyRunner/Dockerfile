FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
EXPOSE 8082

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Svc.StrategyRunner/Svc.StrategyRunner.csproj", "Svc.StrategyRunner/"]
COPY ["FishyLibrary/FishyLibrary.csproj", "FishyLibrary/"]
RUN dotnet restore "Svc.StrategyRunner/Svc.StrategyRunner.csproj"
COPY . .
WORKDIR "/src/Svc.StrategyRunner"
RUN dotnet build "Svc.StrategyRunner.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Svc.StrategyRunner.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Svc.StrategyRunner.dll"]
