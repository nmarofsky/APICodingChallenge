using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace APICodingChallenge.Models
{
    public class APICodingChallengeContext : DbContext
    {
        public APICodingChallengeContext(DbContextOptions<APICodingChallengeContext> options)
            : base(options)
        {
        }

        public DbSet<Player> Players { get; set; } = null!;
        public DbSet<Team> Teams { get; set; } = null!;
    }
}
