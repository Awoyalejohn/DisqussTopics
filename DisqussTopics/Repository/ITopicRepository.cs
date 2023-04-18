using DisqussTopics.Models;

namespace DisqussTopics.Repository
{
    public interface ITopicRepository
    {
        Task<IEnumerable<Topic>> GetTopics();
        IQueryable<Topic> GetTopicsQuery();
        Topic GetTopicBySlug(string slug);
        Task<Topic> GetTopicById(int id);
        void InsertTopic(Topic topic);
        void UpdateTopic(Topic topic);
        void DeleteTopic(string slug);
        Task SaveAsync();
    }
}
