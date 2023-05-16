using DisqussTopics.Models;

namespace DisqussTopics.Repository
{
    public interface ITopicRepository
    {
        Task<IEnumerable<Topic>> GetTopics();
        Task<IEnumerable<Topic>> GetSubscribedTopics(string userId);
        Task<Topic?> GetTopicBySlug(string slug);
        Task<Topic?> GetTopicBySlugNoTrackng(string slug);
        Task<Topic?> GetTopicById(int id);
        Task<Topic?> GetTopicByIdNoTracking(int id);
        Task<Topic?> GetTopicBySlugAsync(string slug);
        void InsertTopic(Topic topic);
        void UpdateTopic(Topic topic);
        void DeleteTopic(Topic topic);
        Task SaveAsync();
    }
}
