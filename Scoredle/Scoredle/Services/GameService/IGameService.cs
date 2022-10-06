using Discord;
using Game = Scoredle.Data.Entities.Game;

namespace Scoredle.Services.GameService
{
    public interface IGameService
    {
        public Task SubmitScore(Game game, IMessage message);
        public Task<List<Game>> GetGames();
        public Task<Game?> GetGameFromMessage(string message);
        public Task<int> LoadHistoricalMessages(IAsyncEnumerable<IReadOnlyCollection<IMessage>> pagedMessages);
    }
}
