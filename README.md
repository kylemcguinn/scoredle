# Scoredle
Work in progress Discord bot for tracking scores for Wordle and Wordle-like games.

## Configuration
Add discord bot token secret via the secret manager
`dotnet user-secrets set "DiscordBotToken" "<insert-token-here>"`

## Debugging
Debug the `Scordle.csproj` using Visual Studio's built in debugger, or alternatively using the .NET CLI
`dotnet run`

## Discord Dev Portal
Authorization link:
https://discord.com/api/oauth2/authorize?client_id=1026292596789747793&permissions=139586825280&scope=bot%20applications.commands