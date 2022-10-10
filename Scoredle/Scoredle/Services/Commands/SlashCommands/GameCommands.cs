using Discord.Interactions;
using Scoredle.Services.GameService;

namespace Scoredle.Services.Commands.SlashCommands
{
    [Group("games", "Commands for interacting with the various game types")]
    public class GameCommands: InteractionModuleBase
    {
        private readonly IGameService _gameService;

        public GameCommands(IGameService gameService)
        {
            _gameService = gameService;
        }
        [SlashCommand("list", "List all games available for scoring")]
        public async Task ListGames()
        {
            var games = await _gameService.GetGames();
            var gameNames = games.Select(x => x.Name);

            var gameResponse = string.Join(Environment.NewLine, gameNames);
            await RespondAsync(gameResponse);
        }
    }
}
