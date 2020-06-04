FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["TwitchSoft.TwitchBot/TwitchSoft.TwitchBot.csproj", "TwitchSoft.TwitchBot/"]
COPY ["TwitchSoft.Shared/TwitchSoft.Shared.csproj", "TwitchSoft.Shared/"]
RUN dotnet restore "TwitchSoft.TwitchBot/TwitchSoft.TwitchBot.csproj"
COPY . .
WORKDIR "/src/TwitchSoft.TwitchBot"
RUN dotnet build "TwitchSoft.TwitchBot.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "TwitchSoft.TwitchBot.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TwitchSoft.TwitchBot.dll"]
