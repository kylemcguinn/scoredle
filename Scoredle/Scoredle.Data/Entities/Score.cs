using System.ComponentModel.DataAnnotations;

namespace Scoredle.Data.Entities
{
    public class Score
    {
        [Key]
        public int Id { get; set; }
        public string UserDisplayName { get; set; } = string.Empty;
        public ulong UserId { get; set; }
        public ulong MessageId { get; set; }
        public int GameId { get; set; }
        public Game Game { get; } = new();
        public int? ScoreValue { get; set; }
        public int? Attempts { get; set; }
        public TimeSpan? TimeValue { get; set; }
        public DateTime ReceivedDateTime { get; set; }
        public DateTimeOffset SubmissionDateTime { get; set; }
        public string SubmissionText { get; set; } = string.Empty;
        public string? Note { get; set; }
        public int? GameNumber { get; set; }
    }
}
