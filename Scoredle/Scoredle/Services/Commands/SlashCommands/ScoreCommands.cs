using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Scoredle.Services.GameService;

namespace Scoredle.Services.Commands.SlashCommands
{
    [Group("score", "Commands for scoring games")]
    public class ScoreCommands : InteractionModuleBase
    {
        private readonly IGameService _gameService;

        public ScoreCommands(IGameService gameService)
        {
            _gameService = gameService;
        }
        [SlashCommand("load-history", "Load historical score messages")]
        public async Task LoadHistorical(
            [Summary(description: "Amount of historical messages to load. If not specified, will load last 100 messages.", name: "message-count")] int? messageCount,
            [ChannelTypes(ChannelType.Text)] ISocketMessageChannel channel)
        {
            var messages = channel.GetMessagesAsync(messageCount ?? 100);
            var scoreCount = await _gameService.LoadHistoricalMessages(messages);

            await RespondAsync($"{scoreCount} scores recorded!");
        }
    }
}
