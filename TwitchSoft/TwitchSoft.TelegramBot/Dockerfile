FROM mcr.microsoft.com/dotnet/core/runtime:3.0.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0.100-buster AS build
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
