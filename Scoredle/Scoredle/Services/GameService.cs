using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing;
using Scoredle.Data;
using Scoredle.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Scoredle.Services
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

        public async Task SubmitScore(ulong messageId, string message, Game game, ulong userId, string userDisplayName, DateTimeOffset submissionDateTime)
        {
            var existingMessage = _scordleContext.Scores.Where(x => x.MessageId == messageId);

            if (existingMessage.ToList().Count > 0)
            {
                Console.WriteLine("Score entry already exists for this message. Skipping score insert!");
                return;
            }

            var match = Regex.Match(message, game.Pattern);

            var attempts = match.Groups["attempts"];
            if (attempts == null)
                throw new Exception("Unable to parse 'attempts' from submission");
            
            var maxAttempts = match.Groups["maxAttempts"];
            if (maxAttempts == null)
                throw new Exception("Unable to parse 'maxAttempts' from submission");

            var hardMode = match.Groups["hard"];
            var gameNumberString = match.Groups["gameNumber"];
            int? gameNumber = null;


            int attemptsValue = int.Parse(attempts.Value);
            int maxAttemptsValue = int.Parse(maxAttempts.Value);

            if (!string.IsNullOrEmpty(gameNumberString.Value))
            {
                gameNumber = int.Parse(gameNumberString.Value);
            }

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
                Attempts = attemptsValue,
                Note = string.IsNullOrEmpty(hardMode.Value) ? null : "hard",
                GameNumber = gameNumber
            };

            _scordleContext.Scores.Add(score);
            await _scordleContext.SaveChangesAsync();
        }

        private int calculateScore(int attempts, int maxAttempts)
        {
            var placement = maxAttempts + 1 - attempts;
            return placement * placement;
        }
    }
}
