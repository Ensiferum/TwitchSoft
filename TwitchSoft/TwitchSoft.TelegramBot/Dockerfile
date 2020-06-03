FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["TwitchSoft.TelegramBot/TwitchSoft.TelegramBot.csproj", "TwitchSoft.TelegramBot/"]
COPY ["TwitchSoft.Shared/TwitchSoft.Shared.csproj", "TwitchSoft.Shared/"]
RUN dotnet restore "TwitchSoft.TelegramBot/TwitchSoft.TelegramBot.csproj"
COPY . .
WORKDIR "/src/TwitchSoft.TelegramBot"
RUN dotnet build "TwitchSoft.TelegramBot.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "TwitchSoft.TelegramBot.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TwitchSoft.TelegramBot.dll"]
