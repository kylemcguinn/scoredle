using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Scoredle.Data.Entities;
using Scoredle.Services.GameService;
using Serilog;
using static System.Formats.Asn1.AsnWriter;

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
            try
            {
                int gameId;
                var success = int.TryParse(gameIdString, out gameId);

                if (!success)
                {
                    Log.Warning("Could not parse Game ID from selected menu item");
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

                var scores = await _gameService.GetScoresBySequentialIdentifier(Context.Guild.Id, Context.Channel.Id, game.Id, currentGameNumber - dayOfTheWeek, currentGameNumber);

                if (scores.Count < 1)
                {
                    await ReplyAsync("No scores available for current week.");
                    return;
                }

                var maxNameLength = scores.Max(x => x.UserDisplayName.Length);
                var format = $"{{0, -{(maxNameLength + 3)}}}{{1, -{dayOfTheWeek * 4}}}{{2, -3}}";
                var scoreResults = string.Format(format, "Name", "Guesses", "Score");

                var scoreGroups = scores.GroupBy(x => x.UserId, (userId, scores) =>
                {
                    var uniqueScores = scores.OrderBy(score => score.SequentialGameIdentifier)
                                .GroupBy(score => score.SequentialGameIdentifier, (id, mScores) => {
                                    var mostRecentScore = mScores.OrderByDescending(x => x.SubmissionDateTime).First();

                                    return new
                                    {
                                        Attempts = mostRecentScore.Attempts,
                                        ScoreValue = mostRecentScore.ScoreValue,
                                        Id = id
                                    };
                                })
                                .Select(score => new { Attempts = score.Attempts, Id = score.Id, ScoreValue = score.ScoreValue });

                    int[] array = new int[dayOfTheWeek + 1];
                    var allScores = new List<string>();

                    for (var i = 0; i < dayOfTheWeek + 1; i++)
                    {
                        string attempts;
                        var score = uniqueScores.SingleOrDefault(x => x.Id == (currentGameNumber - dayOfTheWeek) + i);

                        if (score == null)
                        {
                            attempts = "-";
                        } else
                        {
                            var parsedAttempts = score.Attempts.ToString() ?? "";
                            attempts = string.IsNullOrEmpty(parsedAttempts) ? "X" : parsedAttempts;
                        }

                        allScores.Add(attempts);
                    }

                    return new
                    {
                        Name = scores.First().UserDisplayName,
                        Scores = string.Join(' ', allScores),
                        Total = uniqueScores.Sum(score => score.ScoreValue)
                    };
                })
                .OrderByDescending(x => x.Total);


                foreach (var score in scoreGroups)
                {
                    var scoreString = string.Format(format, score.Name, $"[{score.Scores}]", score.Total);
                    scoreResults += Environment.NewLine + scoreString;
                }

                await ((SocketMessageComponent)Context.Interaction).UpdateAsync(properties =>
                {
                    properties.Content = $"{game.Name}{Environment.NewLine}```{scoreResults}```";
                    properties.Components = null;
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }
    }
}
