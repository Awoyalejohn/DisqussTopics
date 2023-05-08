using DisqussTopics.Data;
using DisqussTopics.Models;
using Microsoft.EntityFrameworkCore;

namespace DisqussTopics.Repository
{
    public class TopicRepository : ITopicRepository
    {
        private readonly DTContext _context;

        public TopicRepository(DTContext context)
        {
            _context = context;
        }

        public void DeleteTopic(Topic topic)
        {
            _context.Topics.Remove(topic);
        }

        public async Task<IEnumerable<Topic>> GetSubscribedTopics(string userId)
        {
            return await _context.Topics
                .Where(t => t.DTUsers.Any(u => u.Id == userId)).ToListAsync();
        }

        public async Task<Topic?> GetTopicById(int id)
        {
            return await _context.Topics
                 .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Topic?> GetTopicByIdNoTracking(int id)
        {
            return await _context.Topics
                .Include(t => t.Posts)
                .Include(t => t.DTUsers)
                .Include(t => t.Rules)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Topic?> GetTopicBySlug(string slug)
        {
            return await _context.Topics
                .Include(t => t.Posts)
                .Include(t => t.DTUsers)
                .Include(t => t.Rules)
                .FirstOrDefaultAsync(t => t.Slug == slug);
        }

        public async Task<Topic?> GetTopicBySlugNoTrackng(string slug)
        {
            return await _context.Topics
                .AsNoTracking()
                .Include(t => t.Posts)
                    .ThenInclude(p => p.DTUser)
                .Include(t => t.Posts)
                    .ThenInclude(p => p.Upvotes)
                .Include(t => t.Posts)
                    .ThenInclude(p => p.Downvotes)
                .Include(t => t.Posts)
                    .ThenInclude(p => p.Comments)
                .Include(t => t.DTUsers)
                .Include(t => t.DTUser)
                .Include(t => t.Rules)
                .FirstOrDefaultAsync(t => t.Slug == slug);
        }

        public async Task<IEnumerable<Topic>> GetTopics()
        {
            var topics = await _context.Topics
                .ToListAsync();

            return topics;
        }

        public void InsertTopic(Topic topic)
        {
            _context.Topics.Add(topic);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void UpdateTopic(Topic topic)
        {
            _context.Update(topic);
        }

    }
}
