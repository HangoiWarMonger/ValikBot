FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Discord.Valik.Api/Discord.Valik.Api.csproj", "Discord.Valik.Api/"]
RUN dotnet restore "Discord.Valik.Api/Discord.Valik.Api.csproj"
COPY . .
WORKDIR "/src/Discord.Valik.Api"
RUN dotnet build "Discord.Valik.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Discord.Valik.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Discord.Valik.Api.dll"]
