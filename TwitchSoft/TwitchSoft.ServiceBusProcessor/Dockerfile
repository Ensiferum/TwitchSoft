FROM mcr.microsoft.com/dotnet/core/runtime:3.0.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0.100-buster AS build
WORKDIR /src
COPY ["TwitchSoft.ServiceBusProcessor/TwitchSoft.ServiceBusProcessor.csproj", "TwitchSoft.ServiceBusProcessor/"]
COPY ["TwitchSoft.Shared/TwitchSoft.Shared.csproj", "TwitchSoft.Shared/"]
RUN dotnet restore "TwitchSoft.ServiceBusProcessor/TwitchSoft.ServiceBusProcessor.csproj"
COPY . .
WORKDIR "/src/TwitchSoft.ServiceBusProcessor"
RUN dotnet build "TwitchSoft.ServiceBusProcessor.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "TwitchSoft.ServiceBusProcessor.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TwitchSoft.ServiceBusProcessor.dll"]
