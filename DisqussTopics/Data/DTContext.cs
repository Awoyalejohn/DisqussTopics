using DisqussTopics.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DisqussTopics.Data
{
    public class DTContext : IdentityDbContext<DTUser>
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
                .WithOne(p => p.Topic)
                .HasForeignKey(p => p.TopicId);
            //.OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Topic>()
                .HasMany(t => t.Rules)
                .WithOne(r => r.Topic)
                .HasForeignKey(r => r.TopicId);
            //.OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Post>()
                .HasMany(p => p.Comments)
                .WithOne(c => c.Post)
                .HasForeignKey(c => c.PostId);
                //.OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Post>()
                .HasMany(p => p.Upvotes)
                .WithMany(d => d.PostUpvotes)
                .UsingEntity(j => j.ToTable("PostUpvotes"));

            modelBuilder.Entity<Post>()
                .HasMany(p => p.Downvotes)
                .WithMany(d => d.PostDownvotes)
                .UsingEntity(j => j.ToTable("PostDownvotes"));
                
            modelBuilder.Entity<Comment>()
                .HasMany(c => c.Upvotes)
                .WithMany(d => d.CommentUpvotes)
                .UsingEntity(j => j.ToTable("CommentUpvotes"));

            modelBuilder.Entity<Comment>()
                .HasMany(c => c.Downvotes)
                .WithMany(d => d.CommentDownvotes)
                .UsingEntity(j => j.ToTable("CommentDownvotes"));

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId);
                //.OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<DTUser>()
                .HasMany(d => d.SubscibedTopics)
                .WithMany(t => t.DTUsers)
                .UsingEntity(j => j.ToTable("UserSubscribedTopics"));

            modelBuilder.Entity<DTUser>()
              .HasMany(d => d.CreatedTopics)
              .WithOne(t => t.DTUser)
              .HasForeignKey(t => t.DTUserId);
            //.OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DTUser>()
                .HasMany(d => d.Comments)
                .WithOne(c => c.DTUser)
                .HasForeignKey(c => c.DTUserId);
            //.OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DTUser>()
                .HasMany(d => d.Posts)
                .WithOne(p => p.DTUser)
                .HasForeignKey(p => p.DTUserId);
                //.OnDelete(DeleteBehavior.Cascade);

        }
    }
}
