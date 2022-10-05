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

        public async Task SubmitScore(string submissionMessaage)
        {
            //_scordleContext.Add<Score>();
        }
    }
}
