#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TwitchSoft.PublicApi/TwitchSoft.PublicApi.csproj", "TwitchSoft.PublicApi/"]
COPY ["TwitchSoft.Shared/TwitchSoft.Shared.csproj", "TwitchSoft.Shared/"]
RUN dotnet restore "TwitchSoft.PublicApi/TwitchSoft.PublicApi.csproj"
COPY . .
WORKDIR "/src/TwitchSoft.PublicApi"
RUN dotnet build "TwitchSoft.PublicApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TwitchSoft.PublicApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TwitchSoft.PublicApi.dll"]