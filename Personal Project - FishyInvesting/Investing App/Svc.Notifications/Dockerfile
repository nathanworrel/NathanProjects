FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Svc.Notifications/Svc.Notifications.csproj", "Svc.Notifications/"]
COPY ["FishyLibrary/FishyLibrary.csproj", "FishyLibrary/"]
RUN dotnet restore "Svc.Notifications/Svc.Notifications.csproj"
COPY . .
WORKDIR "/src/Svc.Notifications"
RUN dotnet build "Svc.Notifications.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Svc.Notifications.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Svc.Notifications.dll"]
