using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Scoredle.Data.Entities;
using Scoredle.Services.GameService;

namespace Scoredle.Services.Commands.SlashCommands
{
    [Group("score", "Commands for scoring games")]
    public class ScoreCommands : InteractionModuleBase<SocketInteractionContext>
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

        [SlashCommand("scoreboard", "Show scoreboard for a game")]
        public async Task Scoreboard()
        {
            var menuBuilder = new SelectMenuBuilder()
                .WithPlaceholder("Select a game")
                .WithCustomId("game-menu-score")
                .WithMinValues(1)
                .WithMaxValues(1);

            var games = await _gameService.GetGames();

            foreach (var game in games)
            {
                menuBuilder.AddOption(game.Name, game.Id.ToString());
            }

            var builder = new ComponentBuilder()
                .WithSelectMenu(menuBuilder);

            await RespondAsync("Select which game to score.", components: builder.Build());
        }

        [ComponentInteraction("game-menu-score", true)]
        public async Task ScoreGame(string gameIdString)
        {
            int gameId;
            var success = int.TryParse(gameIdString, out gameId);

            if (!success)
            {
                // log something here
                return;
            }

            var game = await _gameService.GetGameById(gameId);
            if (game == null)
            {
                await ReplyAsync("Unable to resolve game");
                return;
            }
            else if (!game.Epoch.HasValue)
            {
                await ReplyAsync("Non sequential game identifiers not currently supported");
                return;
            }

            var currentGameNumber = DateTime.Now.Subtract(game.Epoch.Value).Days;
            var dayOfTheWeek = (int)DateTime.Now.DayOfWeek;

            var scores = await _gameService.GetScoresBySequentialIdentifier(game.Id, currentGameNumber - dayOfTheWeek, currentGameNumber);

            var maxNameLength = scores.Max(x => x.UserDisplayName.Length);
            var format = $"{{0, -{(maxNameLength + 5)}}}{{1, -{dayOfTheWeek * 5}}}{{2, -4}}";
            var scoreResults = string.Format(format, "Name", "Guesses", "Total");

            var scoreGroups = scores.GroupBy(x => x.UserId, (userId, scores) => new
            {
                Name = scores.First().UserDisplayName,
                Scores = string.Join(' ', scores.Select(score => score.Attempts)),
                Total = scores.Sum(score => score.ScoreValue)
            });


            foreach (var score in scoreGroups) {
                var scoreString = string.Format(format, score.Name, $"[{score.Scores}]", score.Total);
                scoreResults += Environment.NewLine + scoreString;
            }

            //await ((SocketMessageComponent)Context.Interaction).UpdateAsync(properties =>
            //{
            //    properties.Content = $"```{scoreResults}```";
            //    properties.Components = null;
            //});
            await RespondAsync($"```{scoreResults}```");
        }
    }
}
