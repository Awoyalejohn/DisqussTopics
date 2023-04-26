using DisqussTopics.Models;

namespace DisqussTopics.Repository
{
    public interface IRuleRepository
    {
        void InsertRule(Rule rule);
        void UpdateRule(Rule rule);
        void DeleteRule(Rule rule);

        Task<Rule?> GetRuleById(int id);
        Task<Rule?> GetRuleByIdNoTracking(int id);
        Task SaveAsync();
    }
}
