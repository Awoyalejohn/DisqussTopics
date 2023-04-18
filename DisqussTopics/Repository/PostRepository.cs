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

        public void DeletePost(string slug)
        {
            throw new NotImplementedException();
        }

        public Post GetPostBySlug(string slug)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Post>> GetPosts()
        {
            return await _context.Posts
                .AsNoTracking()
                .Include(p => p.Topic)
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
            throw new NotImplementedException();
        }
    }
}
