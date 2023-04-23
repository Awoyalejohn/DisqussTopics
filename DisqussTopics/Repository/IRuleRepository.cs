using DisqussTopics.Models;

namespace DisqussTopics.Repository
{
    public interface IRuleRepository
    {
        void InsertRule(Rule rule);
        Task SaveAsync();
    }
}
