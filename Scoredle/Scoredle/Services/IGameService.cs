using Scoredle.Data.Entities;

namespace Scoredle.Services
{
    public interface IGameService
    {
        public Task SubmitScore(string submissionMessaage);
        public Task<List<Game>> GetGames();
        public Task<Game?> GetGameFromMessage(string message);
    }
}
