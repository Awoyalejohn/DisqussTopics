using DisqussTopics.Data;
using DisqussTopics.Models;
using Microsoft.EntityFrameworkCore;

namespace DisqussTopics.Repository
{
    public class PostRepository : IPostRepository
    {
        private readonly DTContext _context;

        public PostRepository(DTContext context)
        {
            _context = context;
        }

        public void DeletePost(Post post)
        {
            _context.Posts.Remove(post);
        }

        public Post GetPostBySlug(string slug, string topic)
        {
            throw new NotImplementedException();
        }
        public async Task<Post> GetPostBySlugNoTracking(string slug, string topic)
        {
            return await _context.Posts
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Slug == slug && p.Topic.Name == topic);
        }

        public async Task<Post> GetPostById(int id)
        {
            return await _context.Posts
                .Include(p => p.Topic)
                .Include(p => p.Upvotes)
                .Include(p => p.Downvotes)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<Post> GetPostByIdNoTracking(int id)
        {
            return await _context.Posts
                .AsNoTracking()
                .Include(p => p.DTUser)
                .Include(p => p.Topic)
                    .ThenInclude(t => t.DTUser)
                .Include(p => p.Topic)
                    .ThenInclude(t => t.DTUsers)
                .Include(p => p.Topic)
                    .ThenInclude(t => t.Rules)
                .Include(p => p.Upvotes)
                .Include(p => p.Downvotes)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Post>> GetPostsNoTracking()
        {
            return await _context.Posts
                .AsNoTracking()
                .Include(p => p.Topic)
                .Include(p => p.DTUser)
                .ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetPosts()
        {
            return await _context.Posts
                .Include(p => p.Topic)
                .ThenInclude(t => t.DTUsers)
                .Include(p => p.DTUser)
                .Include(p => p.Upvotes)
                .Include(p => p.Downvotes)
                .Include(p => p.Comments)
                .ToListAsync();
        }

        public void InsertPost(Post post)
        {
            _context.Posts.Add(post);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void UpdatePost(Post post)
        {
            _context.Posts.Update(post);
        }

        public IQueryable<Post> GetPostsQuery()
        {
            var posts = from post in _context.Posts
                        select post;

            return posts;
        }
    }
}
