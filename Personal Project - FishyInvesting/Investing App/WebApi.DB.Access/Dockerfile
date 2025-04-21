FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WebApi.DB.Access/WebApi.DB.Access.csproj", "WebApi.DB.Access/"]
RUN dotnet restore "WebApi.DB.Access/WebApi.DB.Access.csproj"
COPY . .
WORKDIR "/src/WebApi.DB.Access"
RUN dotnet build "WebApi.DB.Access.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "WebApi.DB.Access.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApi.DB.Access.dll"]
