using Discord;
using Scoredle.Data.Entities;
using Game = Scoredle.Data.Entities.Game;

namespace Scoredle.Services.GameService
{
    public interface IGameService
    {
        public Task SubmitScore(Game game, IMessage message);
        public Task<List<Game>> GetGames();
        public Task<Game?> GetGameFromMessage(string message);
        public Task<Game?> GetGameById(int id);
        public Task<int> LoadHistoricalMessages(IAsyncEnumerable<IReadOnlyCollection<IMessage>> pagedMessages);
        public Task<List<Score>> GetScoresBySequentialIdentifier(ulong guildId, ulong channelId, int gameId, int minGameId, int maxGameId);
    }
}
