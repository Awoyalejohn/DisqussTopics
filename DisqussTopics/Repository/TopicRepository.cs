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

        public void DeleteTopic(string slug)
        {
            throw new NotImplementedException();
        }

        public async Task<Topic?> GetTopicById(int id)
        {
           return await _context.Topics
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public Topic GetTopicBySlug(string slug)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Topic>> GetTopics()
        {
            throw new NotImplementedException();
        }

        public IQueryable<Topic> GetTopicsQuery()
        {
            var topics = from topic in _context.Topics
                         select topic;
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
            throw new NotImplementedException();
        }

    }
}
