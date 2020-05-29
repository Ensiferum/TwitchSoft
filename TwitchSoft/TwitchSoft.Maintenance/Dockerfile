FROM mcr.microsoft.com/dotnet/core/runtime:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["TwitchSoft.Maintenance/TwitchSoft.Maintenance.csproj", "TwitchSoft.Maintenance/"]
RUN dotnet restore "TwitchSoft.Maintenance/TwitchSoft.Maintenance.csproj"
COPY . .
WORKDIR "/src/TwitchSoft.Maintenance"
RUN dotnet build "TwitchSoft.Maintenance.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TwitchSoft.Maintenance.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TwitchSoft.Maintenance.dll"]