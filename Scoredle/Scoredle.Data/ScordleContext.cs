using Microsoft.EntityFrameworkCore;
using Scoredle.Data.Entities;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace Scoredle.Data
{
    public class ScordleContext : DbContext
    {
        public ScordleContext(DbContextOptions<ScordleContext> options) : base(options)
        {
            //Database.SetInitializer<SchoolContext>(new SchoolDBInitializer());
        }
        public ScordleContext() { }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(@"Server=localhost; Database=Scordle; Integrated Security=True;");
        //}

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Game>().HasData(new Game
        //    {
        //        Id = 1,
        //        Description = "Wordle is a simple online word game that challenges people to find a five-letter word in six guesses - with a new puzzle being published every day",
        //        Name = "Wordle",
        //        Pattern = "Wordle (?<week>[0-9]+) (?<score>[0-6]*[X]*)/6(?<hard>[*]*)",
        //        Url = "https://www.nytimes.com/games/wordle/index.html"
        //    });
        //}

        //public DbSet<Student> Students { get; set; }
        //public DbSet<Grade> Grades { get; set; }
        //public DbSet<Course> Courses { get; set; }
        //public DbSet<StudentAddress> StudentAddresses { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Score> Scores { get; set; }
    }
}