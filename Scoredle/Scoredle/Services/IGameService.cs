using Scoredle.Data.Entities;

namespace Scoredle.Services
{
    public interface IGameService
    {
        public Task SubmitScore(ulong messageId, string message, Game game, ulong userId, string userDisplayName, DateTimeOffset submissionDateTime);
        public Task<List<Game>> GetGames();
        public Task<Game?> GetGameFromMessage(string message);
    }
}
