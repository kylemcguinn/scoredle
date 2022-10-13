using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Scoredle.Data;
using Scoredle.Data.Entities;
using System.Text.RegularExpressions;
using Game = Scoredle.Data.Entities.Game;

namespace Scoredle.Services.GameService
{
    public class GameService : IGameService
    {
        ScordleContext _scordleContext;
        public GameService(ScordleContext scordleContext)
        {
            _scordleContext = scordleContext;
        }

        public async Task<List<Game>> GetGames()
        {
            var games = await _scordleContext.Games.ToListAsync();

            return games;
        }

        public async Task<Game?> GetGameFromMessage(string message)
        {
            var games = await GetGames();
            var game = getGame(message, games);

            return game;
        }

        public async Task<List<Score>> GetScoresBySequentialIdentifier(ulong guildId, int gameId, int minGameId, int maxGameId)
        {
            var scores = await _scordleContext.Scores
                .Where(x => x.GuildId == guildId && x.GameId == gameId && minGameId <= x.SequentialGameIdentifier && x.SequentialGameIdentifier <= maxGameId)
                .ToListAsync();

            return scores;
        }

        public async Task<Game?> GetGameById(int id)
        {
            var games = await _scordleContext.Games.Where(x => x.Id == id).ToListAsync();

            return games.FirstOrDefault();
        }

        private Game? getGame(string message, List<Game> games)
        {
            Game? matchedGame = null;

            foreach (Game game in games)
            {
                var matches = Regex.Match(message, game.Pattern);

                if (matches.Success)
                {
                    matchedGame = game;
                    break;
                }
            };

            return matchedGame;
        }

        private Score? getScore(Game game, IMessage message)
        {
            var messageId = message.Id;
            var messageContent = message.Content;
            var userId = message.Author.Id;
            string userDisplayName = (message.Author as SocketGuildUser)?.DisplayName ?? message.Author.Username;
            var submissionDateTime = message.Timestamp;
            var channelId = message.Channel.Id;
            var guildId = (message.Channel as SocketGuildChannel)?.Guild?.Id;


            var existingMessage = _scordleContext.Scores.Where(x => x.MessageId == message.Id);

            if (existingMessage.ToList().Count > 0)
            {
                Console.WriteLine("Score entry already exists for this message. Skipping score insert!");
                return null;
            }

            var match = Regex.Match(message.Content, game.Pattern);

            var attempts = match.Groups["attempts"];
            if (attempts == null)
                throw new Exception("Unable to parse 'attempts' from submission");

            var maxAttempts = match.Groups["maxAttempts"];
            if (maxAttempts == null)
                throw new Exception("Unable to parse 'maxAttempts' from submission");

            var hardMode = match.Groups["hard"];
            var gameId = match.Groups["gameId"];

            int sequentialId;
            var isSeqential = int.TryParse(gameId.Value, out sequentialId);

            int attemptsValue;
            int.TryParse(attempts.Value, out attemptsValue);

            int maxAttemptsValue = int.Parse(maxAttempts.Value);

            var score = new Score
            {
                MessageId = messageId,
                GameId = game.Id,
                SubmissionText = match.Value,
                UserId = userId,
                UserDisplayName = userDisplayName,
                SubmissionDateTime = submissionDateTime,
                ReceivedDateTime = DateTime.UtcNow,
                ScoreValue = calculateScore(attemptsValue, maxAttemptsValue),
                Attempts = attemptsValue > 0 ? attemptsValue : null,
                Note = string.IsNullOrEmpty(hardMode.Value) ? null : "hard",
                GameIdentifier = string.IsNullOrEmpty(gameId.Value) ? null : gameId.Value,
                ChannelId = channelId,
                GuildId = guildId,
                SequentialGameIdentifier = isSeqential ? sequentialId : null
            };

            return score;
        }

        public async Task SubmitScore(Game game, IMessage message)
        {
            var score = getScore(game, message);

            if (score == null)
                return;

            _scordleContext.Scores.Add(score);

            await _scordleContext.SaveChangesAsync();
        }

        public async Task<int> LoadHistoricalMessages(IAsyncEnumerable<IReadOnlyCollection<IMessage>> pagedMessages)
        {
            var games = await GetGames();

            await foreach (var messages in pagedMessages)
            {
                foreach (var message in messages)
                {
                    var game = getGame(message.Content, games);
                    
                    if (game != null)
                    {
                        var score = getScore(game, message);

                        if (score != null)
                            _scordleContext.Scores.Add(score);
                    }
                }
            }

            var insertedRows = await _scordleContext.SaveChangesAsync();
            return insertedRows;
        }

        private int calculateScore(int attempts, int maxAttempts)
        {
            if (attempts <= 0)
                return 0;

            var placement = maxAttempts + 1 - attempts;
            return placement * placement;
        }
    }
}
