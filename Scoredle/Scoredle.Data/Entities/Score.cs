using System.ComponentModel.DataAnnotations;

namespace Scoredle.Data.Entities
{
    public class Score
    {
        [Key]
        public int ScoreId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int GameId { get; set; }
        public Game Game { get; } = new();
        public int Value { get; set; }
        public DateTime SubmitDateTime { get; set; }
        public string Submission { get; set; } = string.Empty;
    }
}
