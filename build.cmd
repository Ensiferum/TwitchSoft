docker-compose -f .\TwitchSoft\docker-compose.yml -f .\TwitchSoft\docker-compose.override.yml build

az acr login --name twitchsoft

docker tag twitchsoftmaintenance twitchsoft.azurecr.io/twitchsoftmaintenance:latest
docker tag twitchsofttwitchbot twitchsoft.azurecr.io/twitchsofttwitchbot:latest
docker tag twitchsofttelegrambot twitchsoft.azurecr.io/twitchsofttelegrambot:latest
docker tag twitchsoftservicebusprocessor twitchsoft.azurecr.io/twitchsoftservicebusprocessor:latest

docker push twitchsoft.azurecr.io/twitchsoftmaintenance:latest
docker push twitchsoft.azurecr.io/twitchsofttwitchbot:latest
docker push twitchsoft.azurecr.io/twitchsofttelegrambot:latest
docker push twitchsoft.azurecr.io/twitchsoftservicebusprocessor:latest
