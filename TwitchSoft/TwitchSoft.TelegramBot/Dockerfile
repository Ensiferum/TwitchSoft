FROM mcr.microsoft.com/dotnet/core/runtime:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["TwitchSoft.TelegramBot/TwitchSoft.TelegramBot.csproj", "TwitchSoft.TelegramBot/"]
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
