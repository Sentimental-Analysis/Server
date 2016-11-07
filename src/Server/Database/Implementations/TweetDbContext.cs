using Core.Database.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Server.Database.Implementations
{
    public class TweetDbContext : DbContext, IDbContext
    {
        public DbSet<Tweet> Tweets { get; set; }

        public TweetDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}