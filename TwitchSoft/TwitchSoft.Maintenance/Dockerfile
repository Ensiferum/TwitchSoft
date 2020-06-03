FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["TwitchSoft.Maintenance/TwitchSoft.Maintenance.csproj", "TwitchSoft.Maintenance/"]
COPY ["TwitchSoft.Shared/TwitchSoft.Shared.csproj", "TwitchSoft.Shared/"]
RUN dotnet restore "TwitchSoft.Maintenance/TwitchSoft.Maintenance.csproj"
COPY . .
WORKDIR "/src/TwitchSoft.Maintenance"
RUN dotnet build "TwitchSoft.Maintenance.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "TwitchSoft.Maintenance.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TwitchSoft.Maintenance.dll"]