# TwitchSoft

Pet project to play with some new technologies.
In general, idea was to handle a big amount of data and collect some statistic and history data from Twitch

## Implemented stuff

* Twitch bot
* Telegram bot
* Infrastructure hosting in Azure k8s(AKS)
* Elasticsearch and Kibana to store messages and allow search by text
* GRPC services to communicate (attempt to use GRPC instead of simple Asp.net WebApi)
* RabbitMQ to handle a huge amount of messages in twitch chats
* MediatR to keep code clean
* SignalR used by orchestration service to handle multiple twitch bots connected
* Deployment via Github Actions
* Jobs via Coravel lib
