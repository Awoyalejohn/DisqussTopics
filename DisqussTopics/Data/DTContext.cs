using DisqussTopics.Models;
using Microsoft.EntityFrameworkCore;

namespace DisqussTopics.Data
{
    public class DTContext : DbContext
    {
        public DTContext(DbContextOptions<DTContext> options) : base(options) { }

        public DbSet<Topic> Topics { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Rule> Rules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Topic>()
                .HasMany(t => t.Posts)
                .WithOne(p => p.Topic);

            modelBuilder.Entity<Topic>()
                .HasMany(t => t.Rules)
                .WithOne(r => r.Topic);

            modelBuilder.Entity<Post>()
                .HasMany(t => t.Comments)
                .WithOne(p => p.Post);
        }
    }
}
