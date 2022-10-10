using Discord;
using Discord.WebSocket;
using Scoredle.Services.GameService;

namespace Scoredle.Services.Commands.SlashCommands
{
    public class ListGamesCommand : ICommand<SocketSlashCommand, SlashCommandBuilder>
    {
        private readonly IGameService _gameService;
        public SocketSlashCommand Parameter { get; set; }
        public SlashCommandBuilder Configuration { get; set; }

        public ListGamesCommand(IGameService gameService)
        {
            _gameService = gameService;
        }

        public async Task Execute()
        {
            var games = await _gameService.GetGames();
            await Parameter.RespondAsync($"{games.Count()}");
        }
    }
}
