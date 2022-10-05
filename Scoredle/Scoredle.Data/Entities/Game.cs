using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoredle.Data.Entities
{
    public class Game
    {
        /// <summary>
        /// Unique identifier for the game
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// Name of the game (e.g. "Wordle")
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Pattern to match when reading score messages (written as a regular expression)
        /// </summary>
        public string Pattern { get; set; } = string.Empty;
        /// <summary>
        /// (optional) A brief description of the game
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// (optional) Url used to play the game
        /// </summary>
        public string? Url { get; set; }
    }
}
